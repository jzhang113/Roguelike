using BearLib;
using Roguelike.Core;
using Roguelike.Utils;
using System.Drawing;

namespace Roguelike.Animations
{
    internal class FlashAnimation : IAnimation
    {
        public LayerInfo Layer { get; }
        public int Turn { get; } = Systems.EventScheduler.Turn;

        private const int _MAX_FRAME = 4;

        private readonly int _x;
        private readonly int _y;
        private readonly Color _color;
        private int _frame;

        public FlashAnimation(LayerInfo layer, int x, int y, Color color)
        {
            Layer = layer;
            _x = x;
            _y = y;
            _color = color;
            _frame = 0;
        }

        public bool Update()
        {
            if (_frame >= _MAX_FRAME)
            {
                return true;
            }
            else
            {
                _frame++;
                return false;
            }
        }

        public void Draw()
        {
            Color between = _color.Blend(Colors.Floor, (double)_frame / _MAX_FRAME);
            Terminal.Color(between);
            Terminal.Layer(Layer.Z + 1);
            Layer.Put(_x - Camera.X, _y - Camera.Y, '▓');
            Terminal.Layer(Layer.Z);
        }
    }
}
