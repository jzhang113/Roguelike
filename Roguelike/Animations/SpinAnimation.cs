using BearLib;
using Roguelike.Core;
using System;

namespace Roguelike.Animations
{
    internal class SpinAnimation : IAnimation
    {
        public bool Done { get; private set; }

        private readonly int _x;
        private readonly int _y;
        private int _frame;

        public SpinAnimation(int x, int y)
        {
            _x = x;
            _y = y;
            _frame = 0;
        }

        public void Update()
        {
            System.Diagnostics.Debug.Assert(!Done);

            if (++_frame >= 8)
            {
                Done = true;
                OnComplete(EventArgs.Empty);
            }
        }

        public void Draw(LayerInfo layer)
        {
            Terminal.Color(Swatch.DbBrightWood);
            switch(_frame)
            {
                case 1: layer.Put(_x - 1, _y - 1, '\\'); break;
                case 2: layer.Put(_x - 1, _y, '-'); break;
                case 3: layer.Put(_x - 1, _y + 1, '/'); break;
                case 4: layer.Put(_x, _y + 1, '|'); break;
                case 5: layer.Put(_x + 1, _y + 1, '\\'); break;
                case 6: layer.Put(_x + 1, _y, '-'); break;
                case 7: layer.Put(_x + 1, _y - 1, '/'); break;
                case 8: layer.Put(_x, _y - 1, '|'); break;
            }
        }

        public event EventHandler Complete;
        private void OnComplete(EventArgs e) => Complete?.Invoke(this, e);
    }
}
