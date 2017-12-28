using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using Roguelike.Configurations;
using RogueSharp.Random;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Roguelike
{
    class Game
    {
        public static Configuration Config { get; private set; }
        public static DungeonMap Map { get; private set; }
        public static Player Player { get; private set; }
        public static IRandom Random { get; private set; }

        private static RLRootConsole _rootConsole;
        private static RLConsole _mapConsole;
        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _inventoryConsole;
        
        private static MessageHandler _messageHandler;

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

            Random = new DotNetRandom();

            MapGenerator mapGenerator = new MapGenerator(Config.Map.Width, Config.Map.Height);
            Map = mapGenerator.CreateMap();

            Player = new Player();

            while (!Map.GetCell(Player.X, Player.Y).IsWalkable)
            {
                Player.X = Random.Next(0, Config.Map.Width - 1);
                Player.Y = Random.Next(0, Config.Map.Height - 1);
            }

            Map.UpdatePlayerFov();

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

            _messageHandler = new MessageHandler(Config.MessageMaxCount);

            _rootConsole.Update += RootConsoleUpdate;
            _rootConsole.Render += RootConsoleRender;
            _rootConsole.Run();
        }

        internal static void GameOver()
        {
            _messageHandler.AddMessage("Game Over");
        }

        private static void RootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            ICommand action = InputHandler.HandleInput(_rootConsole);

            if (action != null)
            {
                action.Execute(Player, null);
                _messageHandler.AddMessage(action.Message(Player));
            }
        }

        private static void RootConsoleRender(object sender, UpdateEventArgs e)
        {
            _messageConsole.Clear(0, Swatch.DbDeepWater, Colors.TextHeading);
            _statConsole.Clear(0, Swatch.DbOldStone, Colors.TextHeading);
            _inventoryConsole.Clear(0, Swatch.DbWood, Colors.TextHeading);
            _mapConsole.Clear();

            Map.Draw(_mapConsole);
            Player.Draw(_mapConsole, Map);
            _messageHandler.Draw(_messageConsole);

            RLConsole.Blit(_messageConsole, 0, 0, Config.MessageView.Width, Config.MessageView.Height, _rootConsole, 0, 0);
            RLConsole.Blit(_mapConsole, 0, 0, Config.MapView.Width, Config.MapView.Height, _rootConsole, 0, Config.MessageView.Height);
            RLConsole.Blit(_statConsole, 0, 0, Config.StatView.Width, Config.StatView.Height, _rootConsole, 0, Config.MessageView.Height + Config.MapView.Height);
            RLConsole.Blit(_inventoryConsole, 0, 0, Config.InventoryView.Width, Config.InventoryView.Height, _rootConsole, Config.Map.Width, 0);

            _rootConsole.Draw();
        }
    }
}
