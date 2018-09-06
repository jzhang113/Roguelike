using RLNET;
using Roguelike.Core;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Interfaces
{
    [Serializable]
    class AnimatedDrawable : Drawable
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

        public AnimatedDrawable(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _foreground = (ColorInterval)info.GetValue(nameof(_foreground), typeof(ColorInterval));
            _background = (ColorInterval)info.GetValue(nameof(_background), typeof(ColorInterval));
            _drawBackground = info.GetBoolean(nameof(_drawBackground));
        }

        public override void Draw(RLConsole console, Tile tile)
        {
            if (!tile.IsExplored)
                return;

            if (!Activated)
                return;

            RLColor foreColor = tile.IsVisible
                ? RLColor.Blend(
                    _foreground.GetColor(Game.VisualRandom), Colors.Floor,
                    Math.Min(tile.Light * 1.5f, 1))
                : RLColor.Blend(
                    Color, Colors.Floor,
                    Data.Constants.MIN_VISIBLE_LIGHT_LEVEL);

            RLColor? backColor = null;
            if (_drawBackground && tile.IsVisible)
            {
                backColor = RLColor.Blend(_background.GetColor(Game.VisualRandom),
                   Colors.Floor, tile.Light);
            }

            DrawTile(console, foreColor, backColor, tile.IsVisible, tile.X, tile.Y);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(_foreground), _foreground);
            info.AddValue(nameof(_background), _background);
            info.AddValue(nameof(_drawBackground), _drawBackground);
        }
    }
}
