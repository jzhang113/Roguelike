using RLNET;
using System;
using Roguelike.Core;

namespace Roguelike.Animations
{
    class ExplosionAnimation : IAnimation
    {
        public bool Done { get; private set; }

        private readonly int _x;
        private readonly int _y;
        private readonly int _radius;
        private readonly RLColor _color;
        private int _counter;

        public ExplosionAnimation(int x, int y, int radius, RLColor color)
        {
            _x = x;
            _y = y;
            _radius = radius;
            _color = color;
        }

        public void Update()
        {
            System.Diagnostics.Debug.Assert(!Done);

            _counter++;
            if (_counter >= _radius)
            {
                Done = true;
                OnComplete(EventArgs.Empty);
            }
        }

        public void Draw(RLConsole mapConsole)
        {
            Game.OverlayHandler.ClearForeground();
            foreach (Tile tile in Game.Map.GetTilesInRadius(_x, _y, _counter))
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, _color);
            }
            Game.OverlayHandler.Draw(mapConsole);
        }

        public event EventHandler Complete;
        private void OnComplete(EventArgs e) => Complete?.Invoke(this, e);
    }
}
