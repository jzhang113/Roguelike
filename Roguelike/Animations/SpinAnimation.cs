using Roguelike.Core;
using System;

namespace Roguelike.Animations
{
    class SpinAnimation : IAnimation
    {
        public bool Done { get; private set; }

        int _x;
        int _y;
        int _frame;

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
                Done = true;
        }

        public void Draw()
        {
            switch(_frame)
            {
                case 1: Game.MapConsole.Set(_x - 1, _y - 1, Swatch.DbBrightWood, null, '\\'); break;
                case 2: Game.MapConsole.Set(_x - 1, _y, Swatch.DbBrightWood, null, '-'); break;
                case 3: Game.MapConsole.Set(_x - 1, _y + 1, Swatch.DbBrightWood, null, '/'); break;
                case 4: Game.MapConsole.Set(_x, _y + 1, Swatch.DbBrightWood, null, '|'); break;
                case 5: Game.MapConsole.Set(_x + 1, _y + 1, Swatch.DbBrightWood, null, '\\'); break;
                case 6: Game.MapConsole.Set(_x + 1, _y, Swatch.DbBrightWood, null, '-'); break;
                case 7: Game.MapConsole.Set(_x + 1, _y - 1, Swatch.DbBrightWood, null, '/'); break;
                case 8: Game.MapConsole.Set(_x, _y - 1, Swatch.DbBrightWood, null, '|'); break;
            }
        }

        public event EventHandler Complete;
        protected virtual void OnComplete(EventArgs e) => Complete?.Invoke(this, e);
    }
}
