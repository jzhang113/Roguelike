using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using Roguelike.Configurations;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace Roguelike
{
    class Game
    {
        public static Configuration Config { get; private set; }
        public static DungeonMap Map { get; private set; }
        public static Player Player { get; private set; }
        public static MessageHandler MessageHandler { get; private set; }

        private static RLRootConsole _rootConsole;
        private static RLConsole _mapConsole;
        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _inventoryConsole;
        
        private static bool _render = true;
        public static EventScheduler EventScheduler { get; private set; }

        static void Main(string[] args)
        {
            Config = new Configuration();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build()
                .Bind(Config);

            string consoleTitle = "Roguelike";

            _rootConsole = new RLRootConsole(Config.FontName, Config.Screen.Width, Config.Screen.Height, Config.FontSize, Config.FontSize, 1, consoleTitle);
            _mapConsole = new RLConsole(Config.MapView.Width, Config.MapView.Height);
            _messageConsole = new RLConsole(Config.MessageView.Width, Config.MessageView.Height);
            _statConsole = new RLConsole(Config.StatView.Width, Config.StatView.Height);
            _inventoryConsole = new RLConsole(Config.InventoryView.Width, Config.InventoryView.Height);
            
            _statConsole.Print(1, 1, "Stats", Colors.TextHeading);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);

            //int seed = (int) DateTime.Now.Ticks;
            int seed = 2127758832;
            Random Random = new Random(seed);
            int[] mapSeeds = new int[30];

            using (StreamWriter writer = new StreamWriter("log"))
            {
                writer.WriteLine(seed);

                for (int i = 0; i < mapSeeds.Length; i++)
                {
                    mapSeeds[i] = Random.Next();
                    writer.WriteLine(mapSeeds[i]);
                }
            }

            MapGenerator mapGenerator = new MapGenerator(Config.Map.Width, Config.Map.Height);
            Map = mapGenerator.CreateMap(new Random(mapSeeds[0]));

            Player = new Player();
            int playerX, playerY;

            do
            {
                playerX = Random.Next(0, Config.Map.Width - 1);
                playerY = Random.Next(0, Config.Map.Height - 1);
            } while (!Map.GetCell(playerX, playerY).IsWalkable);

            Map.SetActorPosition(Player, playerX, playerY);

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

            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(20);

            _rootConsole.Update += RootConsoleUpdate;
            _rootConsole.Render += RootConsoleRender;
            _rootConsole.Run();
        }

        internal static void GameOver()
        {
            MessageHandler.AddMessage("Game Over");
        }

        internal static void Exit()
        {
            _rootConsole.Close();
        }

        private static void RootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            ICommand action = InputHandler.HandleInput(_rootConsole);

            if (action != null)
                EventScheduler.Schedule(action.Resolve(Player, null));

            if (EventScheduler.Update())
                _render = true;
        }

        private static void RootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (_render)
            {
                Map.ClearHighlight();

                _messageConsole.Clear(0, Swatch.DbDeepWater, Colors.TextHeading);
                _statConsole.Clear(0, Swatch.DbOldStone, Colors.TextHeading);
                _inventoryConsole.Clear(0, Swatch.DbWood, Colors.TextHeading);
                _mapConsole.Clear();
                
                MessageHandler.Draw(_messageConsole);

                RLConsole.Blit(_messageConsole, 0, 0, Config.MessageView.Width, Config.MessageView.Height, _rootConsole, 0, 0);
                RLConsole.Blit(_statConsole, 0, 0, Config.StatView.Width, Config.StatView.Height, _rootConsole, 0, Config.MessageView.Height + Config.MapView.Height);
                RLConsole.Blit(_inventoryConsole, 0, 0, Config.InventoryView.Width, Config.InventoryView.Height, _rootConsole, Config.Map.Width, 0);

                _render = false;
            }

            Map.Draw(_mapConsole);
            Player.Draw(_mapConsole, Map);
            RLConsole.Blit(_mapConsole, 0, 0, Config.MapView.Width, Config.MapView.Height, _rootConsole, 0, Config.MessageView.Height);
           _rootConsole.Draw();
        }
    }
}
