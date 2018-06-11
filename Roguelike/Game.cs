using RLNET;
using Roguelike.Core;
using Roguelike.Systems;
using System.IO;
using System;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.ComponentModel;
using Roguelike.Utils;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using Roguelike.Items;

namespace Roguelike
{
    static class Game
    {
        public static Enums.Mode GameMode { get; set; }
        public static bool ShowInventory { get; internal set; }
        public static bool ShowEquipment { get; internal set; }
        public static bool ShowOverlay { get; internal set; }

        public static Configuration Config { get; private set; }
        public static Options Option { get; private set; }

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

        public static void Initialize(Configuration configs, Options options)
        {
            Config = configs;
            Option = options;

            string consoleTitle = "Roguelike";

            _rootConsole = new RLRootConsole(Config.FontName, Config.Screen.Width, Config.Screen.Height, Config.FontSize, Config.FontSize, 1, consoleTitle);
            _mapConsole = new RLConsole(Config.MapView.Width, Config.MapView.Height);
            _messageConsole = new RLConsole(Config.MessageView.Width, Config.MessageView.Height);
            _statConsole = new RLConsole(Config.StatView.Width, Config.StatView.Height);
            _inventoryConsole = new RLConsole(Config.InventoryView.Width, Config.InventoryView.Height);
            _viewConsole = new RLConsole(Config.ViewWindow.Width, Config.ViewWindow.Height);

            InputHandler.Initialize(_rootConsole);
            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(20);

            _rootConsole.Update += RootConsoleUpdate;
            _rootConsole.Render += RootConsoleRender;
            _rootConsole.OnLoad += StartGame;
            _rootConsole.OnClosing += SaveGame;
            _rootConsole.Run();
        }

        private static void NewGame()
        {
            // Option.FixedSeed = false;
            Option.Seed = 10;

            int mainSeed;
            if (Option.FixedSeed)
                mainSeed = Option.Seed;
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

            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();
            MapGenerator mapGenerator = new MapGenerator(Config.Map.Width, Config.Map.Height, Random);
            Map = mapGenerator.CreateMapBSP();
            sw.Stop();

            Console.WriteLine("Map generated in: " + sw.Elapsed);

            Player = new Player();
            do
            {
                Player.X = Random.Next(1, Config.Map.Width - 1);
                Player.Y = Random.Next(1, Config.Map.Height - 1);
            }
            while (!Map.Field[Player.X, Player.Y].IsWalkable);

            Map.AddActor(Player);
            // Map.SetActorPosition(Player, playerX, playerY);

            for (int i = 0; i < 3; i++)
            {
                Skeleton s = new Skeleton();
                while (!Map.Field[s.X, s.Y].IsWalkable)
                {
                    s.X = Random.Next(1, Config.Map.Width - 1);
                    s.Y = Random.Next(1, Config.Map.Height - 1);
                    s.Name = "Mook #" + (i + 1);
                }
                Map.AddActor(s);
            }

            Weapon spear = new Weapon("spear", Materials.Wood)
            {
                AttackSpeed = 240,
                Damage = 200,
                MeleeRange = 1.5f,
                ThrowRange = 7,
                X = Player.X - 1,
                Y = Player.Y - 1,
                Color = Swatch.DbBlood
            };
            Map.AddItem(new ItemInfo(spear));

            IAction rangedDamage = new DamageAction(200, new TargetZone(Enums.TargetShape.Ray, range: 10));
            IAction heal = new HealAction(100, new TargetZone(Enums.TargetShape.Self));

            //var lungeSkill = new System.Collections.Generic.List<IAction>()
            //{
            //    new MoveAction(new TargetZone(Enums.TargetShape.Directional)),
            //    new DamageAction(100, new TargetZone(Enums.TargetShape.Directional))
            //};
            //var lungeAction = new Actions.ActionSequence(150, lungeSkill);
            //spear.AddAbility(lungeAction);

            Armor ha = new Armor("heavy armor", Materials.Iron, Enums.ArmorType.Armor)
            {
                AttackSpeed = 1000,
                Damage = 100,
                MeleeRange = 1,
                ThrowRange = 3,
                X = Player.X - 2,
                Y = Player.Y - 3,
                Color = Swatch.DbMetal
            };
            Map.AddItem(new ItemInfo(ha));

            Scroll magicMissile = new Scroll("scroll of magic missile", rangedDamage)
            {
                X = Player.X - 1,
                Y = Player.Y - 2,
                Color = Swatch.DbSun
            };
            Map.AddItem(new ItemInfo(magicMissile));

            Scroll healing = new Scroll("scroll of healing", heal)
            {
                X = Player.X + 1,
                Y = Player.Y + 1,
                Color = Swatch.DbGrass
            };
            Map.AddItem(new ItemInfo(healing));

            GameMode = Enums.Mode.Normal;
        }

        private static void SaveGame(object sender, CancelEventArgs e)
        {
            using (Stream stream = File.OpenWrite(Constants.SAVE_FILE))
            {
                BinaryFormatter serializer = new BinaryFormatter();

                Stream stream2 = File.OpenWrite(Constants.SAVE_FILE + "_player");
                // serializer.Serialize(stream2, Player as Actor);

                // serializer.Serialize(stream, Map);

                stream2.Close();
            }
        }

        private static void StartGame(object sender, EventArgs e)
        {
            if (File.Exists(Constants.SAVE_FILE))
            {
                Console.WriteLine("Reading saved file");
                Stream stream = null, stream2 = null, stream3 = null;

                try
                {
                    BinaryFormatter deserializer = new BinaryFormatter();

                    stream = File.OpenRead(Constants.SAVE_FILE);
                    stream2 = File.OpenRead(Constants.SAVE_FILE + "_player");
                    stream3 = File.OpenRead(Constants.SAVE_FILE + "_events");

                    Player = (Player)deserializer.Deserialize(stream2);
                    Map = (MapHandler)deserializer.Deserialize(stream);

                    foreach (Actor actor in Map.Units)
                        EventScheduler.AddActor(actor);

                    // Option.FixedSeed = false;
                    Option.Seed = 10;

                    int mainSeed;
                    if (Option.FixedSeed)
                        mainSeed = Option.Seed;
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

                    GameMode = Enums.Mode.Normal;


                    NewGame();
                }
                catch (Exception)
                {
                    Console.Error.WriteLine("Load failed");
                    NewGame();
                }
                finally
                {
                    if (stream != null) stream.Dispose();
                    if (stream2 != null) stream2.Dispose();
                    if (stream3 != null) stream3.Dispose();
                }
            }
            else
            {
                NewGame();
            }
        }

        internal static void GameOver()
        {
            MessageHandler.AddMessage("Game Over.", Enums.MessageLevel.Minimal);
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
            {
                _render = true;
            }
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

            if (GameMode == Enums.Mode.Targetting)
                _mapConsole.Print(1, 1, "targetting mode", Colors.TextHeading);

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
                Player.Equipment.Draw(_inventoryConsole);
                RLConsole.Blit(_inventoryConsole, 0, 0, Config.InventoryView.Width, Config.InventoryView.Height, _rootConsole, Config.Map.Width - 10, 0);
            }

            _rootConsole.Draw();
        }
    }
}
