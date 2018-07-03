using System;
using System.Linq;
using RLNET;
using Roguelike.Commands;
using Roguelike.Core;

namespace Roguelike.State
{
    class AutoexploreState : IState
    {
        private static readonly Lazy<AutoexploreState> _instance = new Lazy<AutoexploreState>(() => new AutoexploreState());
        public static AutoexploreState Instance => _instance.Value;
        private static bool _tick;

        private AutoexploreState()
        {
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            _tick = !_tick;
            if (keyPress == null && _tick)
                return null;

            if (Game.Map.Discovered.Any(tile => Game.Map.IsInteresting(tile.X, tile.Y)))
            {
                Game.StateHandler.PopState();
                return null;
            }

            WeightedPoint move = Game.Map.MoveTowardsTarget(Game.Player.X, Game.Player.Y, Game.Map.AutoexploreMap);
            return new MoveCommand(Game.Player, move.X, move.Y);
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            return null;
        }

        public void Update()
        {
            while (Game.EventScheduler.Update())
            {
                // _render = true;
            }
        }

        public void Draw()
        {
            int screenWidth = Game.Config.MapView.Width;
            int screenHeight = Game.Config.MapView.Height;
            int halfWidth = screenWidth / 2;
            int halfHeight = screenHeight / 2;

            // set left and top limits for the camera
            int startX = Math.Max(Game.Player.X - halfWidth, 0);
            int startY = Math.Max(Game.Player.Y - halfHeight, 0);

            // set right and bottom limits for the camera
            startX = Math.Min(Game.Map.Width - screenWidth, startX);
            startY = Math.Min(Game.Map.Height - screenHeight, startY);

            Game.Map.Draw(Game.MapConsole);
            for (int x = 0; x < Game.Config.MapView.Width; x++)
            {
                for (int y = 0; y < Game.Config.MapView.Height; y++)
                {
                    Game.MapConsole.SetBackColor(x, y, new RLColor(0, 0, 1 - Game.Map.AutoexploreMap[x + startX, y + startY] / 30));
                }
            }

            RLConsole.Blit(Game.MapConsole, 0, 0, Game.Config.MapView.Width, Game.Config.MapView.Height, Game.RootConsole, 0, Game.Config.MessageView.Height);
        }
    }
}
