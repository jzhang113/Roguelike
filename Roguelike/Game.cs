using RLNET;
using Roguelike.Core;
using Roguelike.Systems;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Roguelike.Actors;
using Roguelike.Interfaces;

namespace Roguelike
{
    class Game
    {
        public enum Mode { Normal, Inventory, Drop, Equip, Unequip, Apply, Targetting};

        public static Mode GameMode { get; set; }
        public static bool ShowInventory { get; internal set; }
        public static bool ShowEquipment { get; internal set; }
        public static bool ShowOverlay { get; internal set; }

        public static Configuration Config { get; private set; }
        public static OptionHandler Options { get; private set; }

        public static MapHandler Map { get; private set; }
        public static Player Player { get; private set; }
        public static MessageHandler MessageHandler { get; private set; }
        public static EventScheduler EventScheduler { get; private set; }
        public static Random CombatRandom { get; private set; }

        private static RLRootConsole _rootConsole;
        private static RLConsole _mapConsole;
        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _inventoryConsole;
        private static RLConsole _viewConsole;

        private static bool _render = true;

        static void Main(string[] args)
        {
            Config = new Configuration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build()
                .Bind(Config);

            Options = new OptionHandler(args);
            
            string consoleTitle = "Roguelike";

            _rootConsole = new RLRootConsole(Config.FontName, Config.Screen.Width, Config.Screen.Height, Config.FontSize, Config.FontSize, 1, consoleTitle);
            _mapConsole = new RLConsole(Config.MapView.Width, Config.MapView.Height);
            _messageConsole = new RLConsole(Config.MessageView.Width, Config.MessageView.Height);
            _statConsole = new RLConsole(Config.StatView.Width, Config.StatView.Height);
            _inventoryConsole = new RLConsole(Config.InventoryView.Width, Config.InventoryView.Height);
            _viewConsole = new RLConsole(Config.ViewWindow.Width, Config.ViewWindow.Height);
            
            InputHandler.Initialize(_rootConsole);

            // Debugging settings
            Options.FixedSeed = true;
            Options.Seed = 2127758832;
            Options.Verbosity = OptionHandler.MessageLevel.Normal;

            int mainSeed;
            if (Options.FixedSeed)
                mainSeed = Options.Seed;
            else
                mainSeed = (int)DateTime.Now.Ticks;

            Random Random = new Random(mainSeed);
            int[] generatorSeed = new int[31];

            using (StreamWriter writer = new StreamWriter("log"))
            {
                writer.WriteLine(mainSeed);

                for (int i = 0; i < generatorSeed.Length; i++)
                {
                    generatorSeed[i] = Random.Next();
                    writer.WriteLine(generatorSeed[i]);
                }
            }

            CombatRandom = new Random(generatorSeed[30]);
            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(20);

            MapGenerator mapGenerator = new MapGenerator(Config.Map.Width, Config.Map.Height);
            Map = mapGenerator.CreateMap(new Random(generatorSeed[0]));

            Player = new Player();

            while (!Map.GetCell(Player.X, Player.Y).IsWalkable)
            {
                Player.X = Random.Next(0, Config.Map.Width - 1);
                Player.Y = Random.Next(0, Config.Map.Height - 1);
            } 

            Map.AddActor(Player);
            // Map.SetActorPosition(Player, playerX, playerY);

            for (int i = 0; i < 30; i++)
            {
                Skeleton s = new Skeleton();
                while (!Map.GetCell(s.X, s.Y).IsWalkable)
                {
                    s.X = Random.Next(0, Config.Map.Width - 1);
                    s.Y = Random.Next(0, Config.Map.Height - 1);
                }
                Map.AddActor(s);
            }
            
            Items.Spear spear = new Items.Spear(Interfaces.Materials.Wood)
            {
                X = Player.X - 1,
                Y = Player.Y - 1,
                Color = Swatch.DbBlood
            };
            Map.AddItem(spear);

            Items.HeavyArmor ha = new Items.HeavyArmor(Interfaces.Materials.Iron)
            {
                X = Player.X - 2,
                Y = Player.Y - 3,
                Color = Swatch.DbMetal
            };
            Map.AddItem(ha);

            Items.Scroll magicMissile = new Items.Scroll("scroll of magic missile", new Skills.DamageSkill(null, 100, 100), (s, t) => s != t && s.Distance2(t) < 100, 1)
            {
                X = Player.X - 1,
                Y = Player.Y - 2,
                Color = Swatch.DbSun
            };
            Map.AddItem(magicMissile);

            Items.Scroll healing = new Items.Scroll("scroll of healing", new Skills.HealingSkill(null, 100, 100), (s, t) => s == t, 1)
            {
                X = Player.X + 1,
                Y = Player.Y + 1,
                Color = Swatch.DbGrass
            };
            Map.AddItem(healing);

            GameMode = Mode.Normal;

            _rootConsole.Update += RootConsoleUpdate;
            _rootConsole.Render += RootConsoleRender;
            _rootConsole.Run();
        }

        internal static void GameOver()
        {
            MessageHandler.AddMessage("Game Over.", OptionHandler.MessageLevel.None);
        }

        internal static void Exit()
        {
            _rootConsole.Close();
        }

        internal static void ForceRender()
        {
            _render = true;
        }

        private static void RootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            while (EventScheduler.Update())
                _render = true;
        }

        private static void RootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (MessageHandler.Redraw || _render)
            {
                _messageConsole.Clear(0, Swatch.DbDeepWater, Colors.TextHeading);
                MessageHandler.Draw(_messageConsole);
                RLConsole.Blit(_messageConsole, 0, 0, Config.MessageView.Width, Config.MessageView.Height, _rootConsole, 0, 0);
            }

            if (_render)
            {
                Map.ClearHighlight();

                _statConsole.Clear(0, Swatch.DbOldStone, Colors.TextHeading);
                RLConsole.Blit(_statConsole, 0, 0, Config.StatView.Width, Config.StatView.Height, _rootConsole, 0, Config.MessageView.Height + Config.MapView.Height);

                _render = false;
            }

            _mapConsole.Clear();
            Map.Draw(_mapConsole);
            Player.Draw(_mapConsole, Map);

            _viewConsole.Clear(0, Swatch.DbWood, Colors.TextHeading);
            LookHandler.Draw(_viewConsole);

            RLConsole.Blit(_mapConsole, 0, 0, Config.MapView.Width, Config.MapView.Height, _rootConsole, 0, Config.MessageView.Height);
            RLConsole.Blit(_viewConsole, 0, 0, Config.ViewWindow.Width, Config.ViewWindow.Height, _rootConsole, Config.Map.Width, 0);

            if (ShowInventory)
            {
                _inventoryConsole.Clear(0, Colors.FloorBackground, Colors.TextHeading);
                Player.Inventory.Draw(_inventoryConsole);
                RLConsole.Blit(_inventoryConsole, 0, 0, Config.InventoryView.Width, Config.InventoryView.Height, _rootConsole, Config.Map.Width - 10, 0);
            }

            if (ShowEquipment)
            {
                _inventoryConsole.Clear(0, Colors.FloorBackground, Colors.TextHeading);
                Player.Inventory.Draw(_inventoryConsole);
                RLConsole.Blit(_inventoryConsole, 0, 0, Config.InventoryView.Width, Config.InventoryView.Height, _rootConsole, Config.Map.Width - 10, 0);
            }

            _rootConsole.Draw();
        }
    }
}
