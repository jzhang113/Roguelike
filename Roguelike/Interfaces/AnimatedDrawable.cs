using Roguelike.Core;
using Roguelike.Utils;
using System;
using System.Drawing;

namespace Roguelike.Interfaces
{
    [Serializable]
    internal class AnimatedDrawable : Drawable
    {
        private readonly ColorInterval _foreground;
        private readonly ColorInterval _background;
        private readonly bool _drawBackground;

        public AnimatedDrawable(ColorInterval foreground, ColorInterval? background, char symbol)
            : base(foreground.Primary, symbol, false)
        {
            _foreground = foreground;

            if (background != null)
            {
                _background = background.Value;
                _drawBackground = true;
            }
            else
            {
                _drawBackground = false;
            }
        }

        public override void Draw(LayerInfo layer, Tile tile)
        {
            if (!tile.IsExplored)
                return;

            if (!Activated)
                return;

            Color foreColor = tile.IsVisible
                ? _foreground.GetColor(Game.VisualRandom).Blend(Colors.Floor, Math.Min(tile.Light * 1.5f, 1))
                : Color.Blend(Colors.Floor, Data.Constants.MIN_VISIBLE_LIGHT_LEVEL);

            Color? backColor = null;
            if (_drawBackground && tile.IsVisible)
            {
                backColor = _background.GetColor(Game.VisualRandom).Blend(Colors.Floor, tile.Light);
            }

            DrawTile(layer, foreColor, backColor, tile.IsVisible, tile.X, tile.Y);
        }
    }
}
