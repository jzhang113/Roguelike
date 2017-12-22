using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using RogueSharp.Random;

namespace Roguelike
{
    class Game
    {
        private static readonly int _screenHeight = 60;
        private static readonly int _screenWidth = 80;
        private static RLRootConsole _rootConsole;

        private static readonly int _mapHeight = 40;
        private static readonly int _mapWidth = 60;
        private static RLConsole _mapConsole;

        private static readonly int _messageHeight = 11;
        private static readonly int _messageWidth = 60;
        private static RLConsole _messageConsole;

        private static readonly int _statHeight = 9;
        private static readonly int _statWidth = 60;
        private static RLConsole _statConsole;

        private static readonly int _inventoryHeight = 80;
        private static readonly int _inventoryWidth = 20;
        private static RLConsole _inventoryConsole;

        public static DungeonMap Map { get; private set; }
        public static Player Player { get; private set; }
        public static IRandom Random { get; private set; }
        
        private static MessageHandler _messageHandler;
        private static bool _update = true;

        static void Main(string[] args)
        {
            string fontFileName = "terminal8x8.png";
            string consoleTitle = "Roguelike";

            _rootConsole = new RLRootConsole(fontFileName, _screenWidth, _screenHeight, 8, 8, 1, consoleTitle);
            _mapConsole = new RLConsole(_mapWidth, _mapHeight);
            _messageConsole = new RLConsole(_messageWidth, _messageHeight);
            _statConsole = new RLConsole(_statWidth, _statHeight);
            _inventoryConsole = new RLConsole(_inventoryWidth, _inventoryHeight);

            _statConsole.SetBackColor(0, 0, _statWidth, _statHeight, Swatch.DbOldStone);
            _statConsole.Print(1, 1, "Stats", Colors.TextHeading);
            _inventoryConsole.SetBackColor(0, 0, _inventoryWidth, _inventoryHeight, Swatch.DbWood);
            _inventoryConsole.Print(1, 1, "Inventory", Colors.TextHeading);

            Random = new DotNetRandom();

            MapGenerator mapGenerator = new MapGenerator(_mapWidth, _mapHeight);
            Map = mapGenerator.CreateMap();

            Player = new Player();

            while (!Map.GetCell(Player.X, Player.Y).IsWalkable)
            {
                Player.X = Random.Next(0, _mapWidth - 1);
                Player.Y = Random.Next(0, _mapHeight - 1); 
            }

            Map.UpdatePlayerFov();

            _messageHandler = new MessageHandler(100, 5);

            _rootConsole.Update += RootConsoleUpdate;
            _rootConsole.Render += RootConsoleRender;
            _rootConsole.Run();
        }

        private static void RootConsoleUpdate(object sender, UpdateEventArgs e)
        {
            ICommand action = InputHandler.HandleInput(_rootConsole);

            if (action != null)
            {
                action.Execute(Player, null);
                _messageHandler.AddMessage(action.Message(Player));
                _update = true;
            }
        }

        private static void RootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (_update)
            {
                _messageConsole.Clear(0, Swatch.DbDeepWater, RLColor.Green);
                
                Map.Draw(_mapConsole);
                Player.Draw(_mapConsole, Map);
                _messageHandler.Draw(_messageConsole);

                RLConsole.Blit(_messageConsole, 0, 0, _messageWidth, _messageHeight, _rootConsole, 0, 0);
                RLConsole.Blit(_mapConsole, 0, 0, _mapWidth, _mapHeight, _rootConsole, 0, _messageHeight);
                RLConsole.Blit(_statConsole, 0, 0, _statWidth, _statHeight, _rootConsole, 0, _messageHeight + _mapHeight);
                RLConsole.Blit(_inventoryConsole, 0, 0, _inventoryWidth, _inventoryHeight, _rootConsole, _mapWidth, 0);

                _rootConsole.Draw();
                _update = false;
            }
        }
    }
}
