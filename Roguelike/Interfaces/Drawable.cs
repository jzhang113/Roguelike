using MessagePack;
using MessagePack.Formatters;
using RLNET;
using Roguelike.Core;

namespace Roguelike.Interfaces
{
    [MessagePackObject]
    public class Drawable
    {
        [MessagePackFormatter(typeof(TypelessFormatter))]
        [Key(0)]
        public RLColor Color { get; internal set; }
        [Key(1)]
        public char Symbol { get; internal set; }

        [Key(3)]
        internal bool Activated { get; set; }

        [Key(2)]
        private bool _remember;
        [Key(4)]
        private int _rememberX;
        [Key(5)]
        private int _rememberY;

        public Drawable(RLColor color, char symbol, bool remember)
        {
            Color = color;
            Symbol = symbol;
            Activated = true;
            _remember = remember;
        }

        public virtual void Draw(RLConsole console, Tile tile)
        {
            if (!tile.IsExplored)
                return;

            if (!Activated)
                return;

            // Don't blend wall colors
            RLColor color = Color;
            if (!tile.IsWall)
            {
                color = RLColor.Blend(Color, Colors.Floor,
                    tile.IsVisible
                        ? tile.Light
                        : Data.Constants.MIN_VISIBLE_LIGHT_LEVEL);
            }

            DrawTile(console, color, null, tile.IsVisible, tile.X, tile.Y);
        }

        protected void DrawTile(RLConsole console, RLColor foreground, RLColor? background, bool visible, int x, int y)
        {
            int destX = x - Camera.X;
            int destY = y - Camera.Y;

            if (visible)
            {
                console.Set(destX, destY, foreground, background, Symbol);
                _rememberX = x;
                _rememberY = y;
            }
            else if (_remember)
            {
                console.Set(_rememberX - Camera.X, _rememberY - Camera.Y,
                    foreground, background, Symbol);
            }
            else
            {
                console.Set(destX, destY, Colors.Floor, null, '.');
            }
        }
    }
}
