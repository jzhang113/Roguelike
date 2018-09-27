using Roguelike.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Roguelike.Animations
{
    internal class TrailAnimation : IAnimation
    {
        public bool Done { get; private set; }

        private readonly IList<Tile> _path;
        private readonly Color _color;
        private int _counter;

        public TrailAnimation(IEnumerable<Tile> path, Color color)
        {
            _path = path.ToList();
            _color = color;
            _counter = 0;
        }

        public void Update()
        {
            System.Diagnostics.Debug.Assert(!Done);

            _counter += 2;
            if (_counter > _path.Count - 1)
            {
                _counter = _path.Count - 1;
                Done = true;
                OnComplete(EventArgs.Empty);
            }
        }

        public void Draw(LayerInfo layer)
        {
            if (_path.Count == 0)
                return;

            Game.OverlayHandler.ClearForeground();
            Tile tile = _path[_counter];
            Game.OverlayHandler.Set(tile.X, tile.Y, _color);
            Game.OverlayHandler.Draw(layer);
        }

        public event EventHandler Complete;
        private void OnComplete(EventArgs e) => Complete?.Invoke(this, e);
    }
}
