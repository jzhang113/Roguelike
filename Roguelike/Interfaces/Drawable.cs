using BearLib;
using Roguelike.Core;
using Roguelike.Utils;
using System;
using System.Drawing;

namespace Roguelike.Interfaces
{
    [Serializable]
    public class Drawable
    {
        public Color Color { get; internal set; }
        public char Symbol { get; internal set; }

        internal bool Activated { get; set; }

        private bool _remember;
        private int _rememberX;
        private int _rememberY;

        public Drawable(Color color, char symbol, bool remember)
        {
            Color = color;
            Symbol = symbol;
            Activated = true;
            _remember = remember;
        }

        public virtual void Draw(LayerInfo layer, Tile tile)
        {
            if (!tile.IsExplored)
                return;

            if (!Activated)
                return;

            // Don't blend wall colors
            Color color = Color;
            if (!tile.IsWall)
            {
                color = Color.Blend(Colors.Floor,
                    tile.IsVisible
                        ? Math.Min(tile.Light * 1.5f, 1)
                        : Data.Constants.MIN_VISIBLE_LIGHT_LEVEL);
            }

            DrawTile(layer, color, null, tile.IsVisible, tile.X, tile.Y);
        }

        protected void DrawTile(LayerInfo layer, Color foreground, Color? background,
            bool visible, int x, int y)
        {
            int destX = x - Camera.X;
            int destY = y - Camera.Y;

            Terminal.Color(foreground);

            // TODO: can only set backgrounds on 0th layer
            if (background.HasValue)
                Terminal.BkColor(background.Value);

            if (visible)
            {
                layer.Put(destX, destY, Symbol);
                _rememberX = x;
                _rememberY = y;
            }
            else if (_remember)
            {
                layer.Put(_rememberX - Camera.X, _rememberY - Camera.Y, Symbol);
            }
            else
            {
                Terminal.Color(Colors.FloorBackground);
                layer.Put(destX, destY, '.');
            }
        }
    }
}
