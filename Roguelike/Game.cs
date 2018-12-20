using BearLib;
using Pcg;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.State;
using Roguelike.Systems;
using Roguelike.UI;
using Roguelike.World;
using System;
using System.Collections.Generic;
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
        public static Player Player { get; internal set; }

        public static StateHandler StateHandler { get; private set; }
        public static MessagePanel MessageHandler { get; private set; }
        public static EventScheduler EventScheduler { get; private set; }
        public static OverlayHandler OverlayHandler { get; private set; }

        public static MapHandler Map => World.Map;

        internal static bool ShowEquip { get; set; }
        internal static bool ShowInfo { get; set; }
        internal static bool ShowQte { get; set; }

        private static LayerInfo _highlightLayer;
        private static LayerInfo _mapLayer;
        private static LayerInfo _rightLayer;
        private static LayerInfo _fullConsole;
        private static LayerInfo _messageLayer;
        private static LayerInfo _statLayer;
        private static LayerInfo _leftLayer;

        private static bool _exiting;

        public static void Initialize(Configuration configs, Options options)
        {
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
            _statLayer = new LayerInfo("Stats", 1,
                Constants.SIDEBAR_WIDTH + 2, 1,
                Constants.MAPVIEW_WIDTH, Constants.STATUS_HEIGHT);
            _mapLayer = new LayerInfo("Map", 1,
                Constants.SIDEBAR_WIDTH + 2, Constants.STATUS_HEIGHT + 1,
                Constants.MAPVIEW_WIDTH, Constants.MAPVIEW_HEIGHT);
            _messageLayer = new LayerInfo("Message", 1,
                Constants.SIDEBAR_WIDTH + 2, Constants.STATUS_HEIGHT + Constants.MAPVIEW_HEIGHT + 2,
                Constants.MAPVIEW_WIDTH, Constants.MESSAGE_HEIGHT);

            // left panel for look and info
            _leftLayer = new LayerInfo("Look", 1,
                1, 1,
                Constants.SIDEBAR_WIDTH, Constants.SCREEN_HEIGHT);

            // right panel for inventory and equipment
            _rightLayer = new LayerInfo("Inventory", 1,
                Constants.SIDEBAR_WIDTH + Constants.MAPVIEW_WIDTH + 3, 1,
                Constants.SIDEBAR_WIDTH, Constants.SCREEN_HEIGHT);

            // overlay over map
            _highlightLayer = new LayerInfo("Highlight", 3,
                _mapLayer.X, _mapLayer.Y,
                _mapLayer.Width, _mapLayer.Height);

            _fullConsole = new LayerInfo("Full", 11, 0, 0,
                Constants.SCREEN_WIDTH + 2, Constants.SCREEN_HEIGHT + 2);

            Terminal.Set($"window: size={Constants.SCREEN_WIDTH + 2}x{Constants.SCREEN_HEIGHT + 2}," +
                $"cellsize=auto, title='{Config.GameName}';");
            Terminal.Set($"font: {Config.FontName}, size = {Config.FontSize};");
            Terminal.Set("palette.grass: #6daa2c");
            Terminal.Set("input: filter = [keyboard, mouse]");

            StateHandler = new StateHandler(new Dictionary<Type, LayerInfo>
            {
                [typeof(AnimationState)]   = _mapLayer,
                [typeof(ApplyState)]       = _rightLayer,
                [typeof(AutoexploreState)] = _mapLayer,
                [typeof(DropState)]        = _rightLayer,
                [typeof(EquipState)]       = _rightLayer,
                [typeof(InventoryState)]   = _rightLayer,
                [typeof(SubinvState)]      = _rightLayer,
                [typeof(ItemMenuState)]    = _rightLayer,
                [typeof(MenuState)]        = _fullConsole,
                [typeof(NormalState)]      = _mapLayer,
                [typeof(TargettingState)]  = _mapLayer,
                [typeof(TextInputState)]   = _mapLayer,
                [typeof(UnequipState)]     = _rightLayer
            });

            MessageHandler = new MessagePanel(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(16);
            OverlayHandler = new OverlayHandler(_highlightLayer.Width, _highlightLayer.Height);

            _exiting = false;
            ShowEquip = false;

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
                Map.Draw(_mapLayer);
                MessageHandler.Draw(_messageLayer);
                StatPanel.Draw(_statLayer);

                if (ShowInfo)
                    InfoPanel.Draw(_leftLayer);
                else
                    LookPanel.Draw(_leftLayer);

                if (ShowEquip)
                    Player.Equipment.Draw(_rightLayer);
                else
                    Player.Inventory.Draw(_rightLayer);
            }

            StateHandler.Draw();
            Terminal.Refresh();
        }
    }
}
