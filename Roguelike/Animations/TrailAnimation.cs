using Roguelike.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Roguelike.Animations
{
    internal class TrailAnimation : IAnimation
    {
        public LayerInfo Layer { get; }

        private readonly IList<Tile> _path;
        private readonly Color _color;
        private int _counter;

        public TrailAnimation(LayerInfo layer, IEnumerable<Tile> path, Color color)
        {
            Layer = layer;
            _path = path.ToList();
            _color = color;
            _counter = 0;
        }

        public bool Update()
        {
            if (_counter > _path.Count - 1)
            {
                _counter = _path.Count - 1;
                return true;
            }
            else
            {
                _counter += 2;
                return false;
            }
        }

        public void Draw()
        {
            if (_path.Count == 0)
                return;

            Game.Overlay.Clear();
            Tile tile = _path[_counter];
            Game.Overlay.Set(tile.X, tile.Y, _color);
            Game.Overlay.Draw(Layer);
        }
    }
}
