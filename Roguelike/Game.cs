using Pcg;
using RLNET;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Systems;
using Roguelike.World;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Roguelike
{
    static class Game
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

        public static RLRootConsole RootConsole { get; private set; }
        public static ConsoleInfo MapConsole { get; private set; }
        public static ConsoleInfo InventoryConsole { get; private set; }
        public static ConsoleInfo FullConsole { get; private set; }

        private static ConsoleInfo MessageConsole { get; set; }
        private static ConsoleInfo StatConsole { get; set; }
        private static ConsoleInfo ViewConsole { get; set; }
        private static ConsoleInfo MoveConsole { get; set; }

        private static bool _render = true;

        public static void Initialize(Configuration configs, Options options)
        {
            Config = configs;
            Option = options;
            Random = new PcgRandom(Option.FixedSeed ? Option.Seed : (int)DateTime.Now.Ticks);
            VisualRandom = new PcgRandom(Random.Next());

            string consoleTitle = "Roguelike";

            RootConsole = new RLRootConsole(Config.FontName, Config.ScreenWidth, Config.ScreenHeight, Config.FontSize, Config.FontSize, 1, consoleTitle);
            MapConsole = new ConsoleInfo(
                new RLConsole(Config.MapView.Width, Config.MapView.Height),
                0, Config.StatView.Height);
            InventoryConsole = new ConsoleInfo(
                new RLConsole(Config.InventoryView.Width, Config.InventoryView.Height),
                Config.ScreenWidth - configs.InventoryView.Width, 0);
            FullConsole = new ConsoleInfo(
                new RLConsole(Config.ScreenWidth, Config.ScreenHeight),
                0, 0);
            MessageConsole = new ConsoleInfo(
                new RLConsole(Config.MessageView.Width, Config.MessageView.Height),
                0, Config.StatView.Height + Config.MapView.Height);
            StatConsole = new ConsoleInfo(
                new RLConsole(Config.StatView.Width, Config.StatView.Height),
                0, 0);
            ViewConsole = new ConsoleInfo(
                new RLConsole(Config.ViewWindow.Width, Config.ViewWindow.Height),
                Config.MapView.Width, 0);
            MoveConsole = new ConsoleInfo(
                new RLConsole(Config.MoveView.Width, Config.MoveView.Height), // TODO: update config.json
                Config.MapView.Width, Config.ViewWindow.Height);

            StateHandler = new StateHandler(RootConsole);
            MessageHandler = new MessageHandler(Config.MessageMaxCount);
            EventScheduler = new EventScheduler(16);
            OverlayHandler = new OverlayHandler(Config.MapView.Width, Config.MapView.Height);

            RootConsole.Update += RootConsoleUpdate;
            RootConsole.Render += RootConsoleRender;
            RootConsole.OnClosing += SaveGame;
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

            ForceRender();
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

        private static void SaveGame(object sender, CancelEventArgs e)
        {
            if (World == null)
                return;

            using (Stream saveFile = File.OpenWrite(Constants.SAVE_FILE))
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(saveFile, World);
            }
        }

        public static void Run() => RootConsole.Run();

        internal static void GameOver() => MessageHandler.AddMessage("Game Over.", MessageLevel.Minimal);

        internal static void Exit() => RootConsole.Close();

        internal static void ForceRender() => _render = true;

        private static void RootConsoleUpdate(object sender, UpdateEventArgs e) => StateHandler.Update();

        private static void RootConsoleRender(object sender, UpdateEventArgs e)
        {
            if (!_render)
                return;

            if (MessageHandler.Redraw)
            {
                RLConsole console = MessageConsole.Console;
                console.Clear(0, RLColor.Black, Colors.Text);
                MessageHandler.Draw(console);
                RLConsole.Blit(console, 0, 0, console.Width, console.Height, RootConsole,
                    MessageConsole.X, MessageConsole.Y);
            }

            if (Player != null)
            {
                RLConsole statConsole = StatConsole.Console;
                statConsole.Clear(0, RLColor.Black, Colors.Text);
                InfoHandler.Draw(statConsole);
                RLConsole.Blit(statConsole, 0, 0, statConsole.Width, statConsole.Height, RootConsole,
                    StatConsole.X, StatConsole.Y);

                Items.Weapon weapon = Player.Equipment.PrimaryWeapon;
                RLConsole attackConsole = MoveConsole.Console;
                attackConsole.Clear(0, RLColor.Black, Colors.Text);
                weapon?.Moveset.Draw(attackConsole);
                RLConsole.Blit(attackConsole, 0, 0, attackConsole.Width, attackConsole.Height, RootConsole,
                    MoveConsole.X, MoveConsole.Y);
            }

            RLConsole lookConsole = ViewConsole.Console;
            lookConsole.Clear(0, RLColor.Black, Colors.Text);
            LookHandler.Draw(lookConsole);
            RLConsole.Blit(lookConsole, 0, 0, lookConsole.Width, lookConsole.Height, RootConsole,
                ViewConsole.X, ViewConsole.Y);

            StateHandler.Draw();
            RootConsole.Draw();
            _render = false;
        }
    }
}
