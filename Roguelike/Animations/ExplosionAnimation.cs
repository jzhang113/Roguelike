using Roguelike.Core;
using System.Drawing;

namespace Roguelike.Animations
{
    internal class ExplosionAnimation : IAnimation
    {
        public LayerInfo Layer { get; }
        public int Turn { get; } = Systems.EventScheduler.Turn;

        private readonly Loc _pos;
        private readonly int _radius;
        private readonly Color _color;
        private int _counter;

        public ExplosionAnimation(LayerInfo layer, in Loc pos, int radius, Color color)
        {
            Layer = layer;
            _pos = pos;
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
            foreach (Loc point in Game.Map.GetPointsInRadius(_pos, _counter))
            {
                Game.Overlay.Set(point.X, point.Y, _color);
            }
            Game.Overlay.Draw(Layer);
        }
    }
}
