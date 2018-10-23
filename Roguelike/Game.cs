using BearLib;
using Pcg;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Systems;
using Roguelike.World;
using System;
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
            
            if (!Terminal.Open())
            {
                System.Diagnostics.Debug.WriteLine("Failed to initialize terminal");
                return;
            }

            // main UI elements
            MapLayer = new LayerInfo("Map", 1,
                Constants.SIDEBAR_WIDTH, Constants.STATUS_HEIGHT,
                Constants.MAPVIEW_WIDTH, Constants.MAPVIEW_HEIGHT);
            MessageLayer = new LayerInfo("Message", 1,
                Constants.SIDEBAR_WIDTH, Constants.STATUS_HEIGHT + Constants.MAPVIEW_HEIGHT,
                Constants.MAPVIEW_WIDTH, Constants.MESSAGE_HEIGHT);
            StatLayer = new LayerInfo("Stats", 1,
                Constants.SIDEBAR_WIDTH, 0,
                Constants.MAPVIEW_WIDTH, Constants.STATUS_HEIGHT);
            LookLayer = new LayerInfo("Look", 1,
                0, 0,
                Constants.SIDEBAR_WIDTH, Constants.SCREEN_HEIGHT);
            InventoryLayer = new LayerInfo("Inventory", 1,
                Constants.SIDEBAR_WIDTH + Constants.MAPVIEW_WIDTH, 0,
                Constants.SIDEBAR_WIDTH, Constants.SCREEN_HEIGHT);

            // overlay over map
            HighlightLayer = new LayerInfo("Highlight", 2,
                MapLayer.X, MapLayer.Y,
                MapLayer.Width, MapLayer.Height);

            // alternate tab for inventory
            MoveLayer = new LayerInfo("Moves", 2,
                InventoryLayer.X, InventoryLayer.Y,
                InventoryLayer.Width, InventoryLayer.Height);

            FullConsole = new LayerInfo("Full", 10, 0, 0,
                Constants.SCREEN_WIDTH, Constants.SCREEN_HEIGHT);

            Terminal.Set(
                $"window: size={Constants.SCREEN_WIDTH}x{Constants.SCREEN_HEIGHT}," +
                $"cellsize=auto, title='{Config.GameName}';" +
                $"font: ccc12x12.png, size = 12x12;" +
                $"input: filter = [keyboard, mouse]");

            StateHandler = new StateHandler();
            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(16);
            OverlayHandler = new OverlayHandler(HighlightLayer.Width, HighlightLayer.Height);

            Render();
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
            while (!_exiting)
            {
                StateHandler.Update();
                Render();
            }

            Terminal.Close();
        }

        internal static void Exit()
        {
            SaveGame();
            _exiting = true;
        }

        internal static void GameOver() => MessageHandler.AddMessage("Game Over.", MessageLevel.Minimal);

        internal static void Render()
        {
            Terminal.Clear();

            Terminal.Layer(1);
            if (Player != null)
            {
                MessageHandler.Draw(MessageLayer);
                InfoHandler.Draw(StatLayer);
                LookHandler.Draw(LookLayer);

                Player.Inventory.Draw(InventoryLayer);
                Items.Weapon weapon = Player.Equipment.PrimaryWeapon;
                weapon?.Moveset.Draw(MoveLayer);
            }

            StateHandler.Draw();
            Terminal.Refresh();
        }
    }
}
