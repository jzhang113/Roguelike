using BearLib;
using Roguelike.Core;
using System;

namespace Roguelike.Animations
{
    internal class SpinAnimation : IAnimation
    {
        public LayerInfo Layer { get; }

        private readonly int _x;
        private readonly int _y;
        private int _frame;

        public SpinAnimation(LayerInfo layer, int x, int y)
        {
            Layer = layer;
            _x = x;
            _y = y;
            _frame = 0;
        }

        public bool Update()
        {
            if (_frame >= 8)
                return true;

            _frame++;
            return false;
        }

        public void Draw()
        {
            Terminal.Color(Swatch.DbBrightWood);
            switch(_frame)
            {
                case 1: Layer.Put(_x - 1 - Camera.X, _y - 1 - Camera.Y, '\\'); break;
                case 2: Layer.Put(_x - 1 - Camera.X, _y     - Camera.Y, '-'); break;
                case 3: Layer.Put(_x - 1 - Camera.X, _y + 1 - Camera.Y, '/'); break;
                case 4: Layer.Put(_x     - Camera.X, _y + 1 - Camera.Y, '|'); break;
                case 5: Layer.Put(_x + 1 - Camera.X, _y + 1 - Camera.Y, '\\'); break;
                case 6: Layer.Put(_x + 1 - Camera.X, _y     - Camera.Y, '-'); break;
                case 7: Layer.Put(_x + 1 - Camera.X, _y - 1 - Camera.Y, '/'); break;
                case 8: Layer.Put(_x     - Camera.X, _y - 1 - Camera.Y, '|'); break;
            }
        }
    }
}
