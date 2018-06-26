using RLNET;
using Roguelike.Core;
using Roguelike.Systems;
using System.IO;
using System;
using Roguelike.Actors;
using Roguelike.Utils;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using Roguelike.State;

namespace Roguelike
{
    static class Game
    {
        // internal so input handler can change modes
        public static IState GameState { get; set; }
        public static bool ShowModal { get; internal set; }
        public static bool ShowEquipment { get; internal set; }
        public static bool ShowOverlay { get; internal set; }

        public static Configuration Config { get; private set; }
        public static Options Option { get; private set; }

        public static WorldHandler World { get; private set; }
        public static Player Player { get; internal set; } // internal for deserialization
        public static MessageHandler MessageHandler { get; private set; }
        public static EventScheduler EventScheduler { get; private set; }
        public static StateHandler StateHandler { get; private set; }

        public static MapHandler Map => World.Map;

        public static RLRootConsole _rootConsole;
        public static RLConsole _mapConsole;
        public static RLConsole _messageConsole;
        public static RLConsole _statConsole;
        public static RLConsole _inventoryConsole;
        public static RLConsole _viewConsole;

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

            StateHandler = new StateHandler(_rootConsole);
            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(20);
            World = Option.FixedSeed
                ? new WorldHandler(Option.Seed)
                : new WorldHandler();

            _rootConsole.Update += RootConsoleUpdate;
            _rootConsole.Render += RootConsoleRender;
            _rootConsole.OnLoad += StartGame;
            _rootConsole.OnClosing += SaveGame;
            _rootConsole.Run();
        }

        public static void NewGame()
        {
            Player = new Player(new ActorParameters("Player")
            {
                Awareness = 10,
                MaxHp = 100,
                MaxMp = 50,
                MaxSp = 50
            });
            MessageHandler.Clear();
            EventScheduler.Clear();
            World = Option.FixedSeed
                ? new WorldHandler(Option.Seed)
                : new WorldHandler();

            World.ChangeLevel("main_1");
        }

        private static void SaveGame(object sender, CancelEventArgs e)
        {
            using (Stream saveFile = File.OpenWrite(Constants.SAVE_FILE))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFile, new SaveObject
                {
                    GameState = GameState,
                    ShowEquipment = ShowEquipment,
                    ShowInventory = ShowModal,
                    ShowOverlay = ShowOverlay,
                    World = World
                });
            }
        }

        private static void StartGame(object sender, EventArgs e)
        {
            if (!File.Exists(Constants.SAVE_FILE))
                NewGame();

            System.Diagnostics.Debug.WriteLine("Reading saved file");
            try
            {
                using (Stream saveFile = File.OpenRead(Constants.SAVE_FILE))
                {
                    BinaryFormatter deserializer = new BinaryFormatter();
                    SaveObject saved = (SaveObject)deserializer.Deserialize(saveFile);

                    GameState = saved.GameState;
                    ShowEquipment = saved.ShowEquipment;
                    ShowModal = saved.ShowInventory;
                    ShowOverlay = saved.ShowOverlay;
                    World = saved.World;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load failed: {ex.Message}");
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
            StateHandler.Update();
        }

        private static void RootConsoleRender(object sender, UpdateEventArgs e)
        {
            StateHandler.Draw();

            //if (ShowOverlay)
            //{
            //    OverlayHandler.Draw(_mapConsole);
            //    RLConsole.Blit(_mapConsole, 0, 0, Config.MapView.Width, Config.MapView.Height, _rootConsole, 0, Config.MessageView.Height);
            //}

            //if (ShowModal)
            //{
            //    _inventoryConsole.Clear(0, Colors.FloorBackground, Colors.TextHeading);
            //    Player.Inventory.Draw(_inventoryConsole);
            //    RLConsole.Blit(_inventoryConsole, 0, 0, Config.InventoryView.Width, Config.InventoryView.Height, _rootConsole, Config.Map.Width - 10, 0);
            //}

            //if (ShowEquipment)
            //{
            //    _inventoryConsole.Clear(0, Colors.FloorBackground, Colors.TextHeading);
            //    Player.Equipment.Draw(_inventoryConsole);
            //    RLConsole.Blit(_inventoryConsole, 0, 0, Config.InventoryView.Width, Config.InventoryView.Height, _rootConsole, Config.Map.Width - 10, 0);
            //}

            _rootConsole.Draw();
        }
    }
}
