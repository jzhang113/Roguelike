using MessagePack;
using RLNET;
using Roguelike.Core;
using System;

namespace Roguelike.Interfaces
{
    [MessagePackObject]
    class AnimatedDrawable : Drawable
    {
        [Key(0)]
        private readonly ColorInterval _foreground;
        [Key(1)]
        private readonly ColorInterval _background;
        [Key(2)]
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

        public override void Draw(RLConsole console, Tile tile)
        {
            if (!tile.IsExplored)
                return;

            if (!Activated)
                return;

            RLColor foreColor = tile.IsVisible
                ? RLColor.Blend(_foreground.GetColor(Game.VisualRandom), Colors.Floor, tile.Light)
                : RLColor.Blend(Color, Colors.Floor, Data.Constants.MIN_VISIBLE_LIGHT_LEVEL);

            RLColor? backColor = null;
            if (_drawBackground && tile.IsVisible)
                backColor = RLColor.Blend(_background.GetColor(Game.VisualRandom),
                    Colors.Floor, tile.Light);

            DrawTile(console, foreColor, backColor, tile.IsVisible, tile.X, tile.Y);
        }
    }
}
