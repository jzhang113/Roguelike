using RLNET;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.State;
using Roguelike.Systems;
using Roguelike.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

        public static RLRootConsole RootConsole { get; private set; }
        public static RLConsole MapConsole { get; private set; }
        public static RLConsole InventoryConsole { get; private set; }

        private static RLConsole _messageConsole;
        private static RLConsole _statConsole;
        private static RLConsole _viewConsole;

        private static bool _render = true;

        public static void Initialize(Configuration configs, Options options)
        {
            Config = configs;
            Option = options;

            string consoleTitle = "Roguelike";

            RootConsole = new RLRootConsole(Config.FontName, Config.Screen.Width, Config.Screen.Height, Config.FontSize, Config.FontSize, 1, consoleTitle);
            MapConsole = new RLConsole(Config.MapView.Width, Config.MapView.Height);
            InventoryConsole = new RLConsole(Config.InventoryView.Width, Config.InventoryView.Height);
            _messageConsole = new RLConsole(Config.MessageView.Width, Config.MessageView.Height);
            _statConsole = new RLConsole(Config.StatView.Width, Config.StatView.Height);
            _viewConsole = new RLConsole(Config.ViewWindow.Width, Config.ViewWindow.Height);

            StateHandler = new StateHandler(RootConsole);
            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(20);
            World = Option.FixedSeed
                ? new WorldHandler(Option.Seed)
                : new WorldHandler();

            RootConsole.Update += RootConsoleUpdate;
            RootConsole.Render += RootConsoleRender;
            RootConsole.OnLoad += StartGame;
            RootConsole.OnClosing += SaveGame;
            RootConsole.Run();
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
            ForceRender();
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
            RootConsole.Close();
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
            if (_render)
            {
                Map.ClearHighlight();
                MapConsole.Clear(0, RLColor.Black, Colors.TextHeading, 0);
                Map.Draw(MapConsole);
                RLConsole.Blit(MapConsole, 0, 0, Config.MapView.Width, Config.MapView.Height, RootConsole, 0, Config.MessageView.Height);
            }

            if (MessageHandler.Redraw || _render)
            {
                _messageConsole.Clear(0, Swatch.DbDeepWater, Colors.TextHeading);
                MessageHandler.Draw(_messageConsole);
                RLConsole.Blit(_messageConsole, 0, 0, Config.MessageView.Width, Config.MessageView.Height, RootConsole, 0, 0);

                _render = false;
            }

            _statConsole.Clear(0, Swatch.DbOldStone, Colors.TextHeading);
            RLConsole.Blit(_statConsole, 0, 0, Config.StatView.Width, Config.StatView.Height, RootConsole, 0, Config.MessageView.Height + Config.MapView.Height);

            _viewConsole.Clear(0, Swatch.DbWood, Colors.TextHeading);
            LookHandler.Draw(_viewConsole);
            RLConsole.Blit(_viewConsole, 0, 0, Config.ViewWindow.Width, Config.ViewWindow.Height, RootConsole, Config.MapView.Width, 0);

            StateHandler.Draw();
            RootConsole.Draw();
        }
    }
}
