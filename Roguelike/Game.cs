using BearLib;
using Pcg;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Systems;
using Roguelike.World;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Roguelike
{
    public static class Game
    {
        public static Configuration Config { get; private set; }

        public static Options Option { get; private set; }
        public static PcgRandom Random { get; private set; }
        public static PcgRandom VisualRandom { get; private set; }

        public static WorldHandler World { get; private set; }
        public static Player Player { get; internal set; } // internal for deserialization

        public static StateHandler StateHandler { get; private set; }
        public static MessageHandler MessageHandler { get; private set; }
        public static EventScheduler EventScheduler { get; private set; }
        public static OverlayHandler OverlayHandler { get; private set; }

        public static MapHandler Map => World.Map;
        
        internal static LayerInfo HighlightLayer { get; set; }
        internal static LayerInfo MapLayer { get; set; }
        internal static LayerInfo InventoryLayer { get; set; }
        internal static LayerInfo FullConsole { get; set; }

        private static LayerInfo MessageLayer { get; set; }
        private static LayerInfo StatLayer { get; set; }
        private static LayerInfo LookLayer { get; set; }
        private static LayerInfo MoveLayer { get; set; }

        private static bool _exiting;

        public static void Initialize(Configuration configs, Options options)
        {
            _exiting = false;

            Config = configs;
            Option = options;
            Random = new PcgRandom(Option.FixedSeed ? Option.Seed : (int)DateTime.Now.Ticks);
            VisualRandom = new PcgRandom(Random.Next());

            const string consoleTitle = "Roguelike";

            if (!Terminal.Open())
            {
                Console.WriteLine("Failed to initialize terminal");
                return;
            }

            Terminal.Set(
                $"window: size={Config.ScreenWidth}x{Config.ScreenHeight}," +
                $"cellsize=auto, title='{consoleTitle}';" +
                $"font: ccc12x12.png, size = 12x12;" +
                $"input: filter = [keyboard, mouse]");

            HighlightLayer = new LayerInfo(2,
                0, Config.StatView.Height,
                Config.MapView.Width, configs.MapView.Height);
            MapLayer = new LayerInfo(1,
                0, Config.StatView.Height,
                Config.MapView.Width, Config.MapView.Height);
            InventoryLayer = new LayerInfo(3,
                Config.ScreenWidth - configs.InventoryView.Width, 0,
                Config.InventoryView.Width, Config.InventoryView.Height);
            MessageLayer = new LayerInfo(4,
                0, Config.StatView.Height + Config.MapView.Height,
                Config.MessageView.Width, Config.MessageView.Height);
            StatLayer = new LayerInfo(5,
                0, 0,
                Config.StatView.Width, Config.StatView.Height);
            LookLayer = new LayerInfo(6,
                Config.MapView.Width, 0,
                Config.ViewWindow.Width, Config.ViewWindow.Height);
            MoveLayer = new LayerInfo(7,
                Config.MapView.Width, Config.ViewWindow.Height,
                Config.MoveView.Width, Config.MoveView.Height);

            FullConsole = new LayerInfo(10,
                0, 0,
                Config.ScreenWidth, Config.ScreenHeight);

            StateHandler = new StateHandler();
            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(16);
            OverlayHandler = new OverlayHandler(Config.MapView.Width, Config.MapView.Height);
            
            // TODO: save on closing
        }

        public static void NewGame()
        {
            Random = new PcgRandom(Option.FixedSeed ? Option.Seed : (int)DateTime.Now.Ticks);
            VisualRandom = new PcgRandom(Random.Next());

            Player = new Player(new ActorParameters("Player")
            {
                Awareness = 10,
                MaxHp = 100,
                MaxMp = 50,
                MaxSp = 50
            });

            StateHandler.Reset();
            MessageHandler.Clear();
            EventScheduler.Clear();
            OverlayHandler.ClearBackground();
            OverlayHandler.ClearForeground();

            WorldParameter worldParameter = Program.LoadData<WorldParameter>("world");
            World = new WorldHandler(worldParameter);
            World.Initialize();

            Render();
        }

        public static void LoadGame()
        {
            if (!File.Exists(Constants.SAVE_FILE))
                NewGame();

            System.Diagnostics.Debug.WriteLine("Reading saved file");
            try
            {
                using (Stream saveFile = File.OpenRead(Constants.SAVE_FILE))
                {
                    BinaryFormatter deserializer = new BinaryFormatter();
                    World = (WorldHandler)deserializer.Deserialize(saveFile);
                }

                StateHandler.Reset();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Load failed: {ex.Message}");
                NewGame();
            }
        }

        private static void SaveGame()
        {
            if (World == null)
                return;

            using (Stream saveFile = File.OpenWrite(Constants.SAVE_FILE))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFile, World);
            }
        }

        public static void Run()
        {
            while (true)
            {
                StateHandler.Update();

                if (_exiting)
                    break;

                Render();
            }

            SaveGame();
            Terminal.Close();
        }

        internal static void Exit() => _exiting = true;

        internal static void GameOver() => MessageHandler.AddMessage("Game Over.", MessageLevel.Minimal);

        internal static void Render()
        {
            Terminal.Clear();

            // if (MessageHandler.Redraw)
            {
                Terminal.Layer(MessageLayer.Z);
                MessageHandler.Draw(MessageLayer);
            }

            if (Player != null)
            {
                Terminal.Layer(StatLayer.Z);
                InfoHandler.Draw(StatLayer);

                Items.Weapon weapon = Player.Equipment.PrimaryWeapon;
                Terminal.Layer(MoveLayer.Z);
                weapon?.Moveset.Draw(MoveLayer);
            }

            Terminal.Layer(LookLayer.Z);
            LookHandler.Draw(LookLayer);

            StateHandler.Draw();
            Terminal.Refresh();
        }
    }
}
