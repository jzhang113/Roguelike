using Roguelike.Core;
using System.Drawing;

namespace Roguelike.Animations
{
    internal class ExplosionAnimation : IAnimation
    {
        public LayerInfo Layer { get; }
        public int Turn { get; } = Systems.EventScheduler.Turn;

        private readonly int _x;
        private readonly int _y;
        private readonly int _radius;
        private readonly Color _color;
        private int _counter;

        public ExplosionAnimation(LayerInfo layer, int x, int y, int radius, Color color)
        {
            Layer = layer;
            _x = x;
            _y = y;
            _radius = radius;
            _color = color;
        }

        public bool Update()
        {
            if (_counter >= _radius)
                return true;

            _counter++;
            return false;
        }

        public void Draw()
        {
            Game.Overlay.Clear();
            foreach (Tile tile in Game.Map.GetTilesInRadius(_x, _y, _counter))
            {
                Game.Overlay.Set(tile.X, tile.Y, _color);
            }
            Game.Overlay.Draw(Layer);
        }
    }
}
