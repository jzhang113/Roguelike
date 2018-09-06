using RLNET;
using Roguelike.Core;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Interfaces
{
    [Serializable]
    public class Drawable : ISerializable
    {
        public RLColor Color { get; internal set; }
        public char Symbol { get; internal set; }

        internal bool Activated { get; set; }

        private bool _remember;
        private int _rememberX;
        private int _rememberY;

        public Drawable(RLColor color, char symbol, bool remember)
        {
            Color = color;
            Symbol = symbol;
            Activated = true;
            _remember = remember;
        }

        protected Drawable(SerializationInfo info, StreamingContext context)
        {
            float r = (float)info.GetValue($"{nameof(Color)}.r", typeof(float));
            float g = (float)info.GetValue($"{nameof(Color)}.g", typeof(float));
            float b = (float)info.GetValue($"{nameof(Color)}.b", typeof(float));
            Color = new RLColor(r, g, b);

            Symbol = info.GetChar(nameof(Symbol));
            Activated = info.GetBoolean(nameof(Activated));

            _remember = info.GetBoolean(nameof(_remember));
            _rememberX = info.GetInt32(nameof(_rememberX));
            _rememberY = info.GetInt32(nameof(_rememberY));
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
                        ? Math.Min(tile.Light * 1.5f, 1)
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
                console.Set(destX, destY, Colors.FloorBackground, null, '.');
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(Color)}.r", Color.r);
            info.AddValue($"{nameof(Color)}.g", Color.g);
            info.AddValue($"{nameof(Color)}.b", Color.b);

            info.AddValue(nameof(Symbol), Symbol);
            info.AddValue(nameof(Activated), Activated);

            info.AddValue(nameof(_remember), _remember);
            info.AddValue(nameof(_rememberX), _rememberX);
            info.AddValue(nameof(_rememberY), _rememberY);
        }
    }
}
