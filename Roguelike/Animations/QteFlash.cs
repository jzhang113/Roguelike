using Roguelike.Core;
using Roguelike.UI;
using Roguelike.Utils;
using System;
using System.Drawing;

namespace Roguelike.Animations
{
    internal class QteFlash : IAnimation
    {
        public bool Done { get; private set; }

        private const int _MAX_TICKS = 15;

        private double _ticks;
        private readonly QtePanel _panel;
        private readonly Color _color;

        public QteFlash(QtePanel panel, Color color)
        {
            _ticks = _MAX_TICKS;
            _panel = panel;
            _color = color;
        }

        public void Update()
        {
            System.Diagnostics.Debug.Assert(!Done);

            if (_ticks <= 0)
            {
                Done = true;
                OnComplete(EventArgs.Empty);
            }
            else
            {
                _ticks--;
            }
        }

        public void Draw(LayerInfo layer)
        {
            // TODO: animations only occur on _mapLayer
            _panel.Draw(layer, _color.Blend(Colors.FloorBackground, _ticks / _MAX_TICKS));
        }

        public event EventHandler Complete;
        private void OnComplete(EventArgs e) => Complete?.Invoke(this, e);
    }
}
