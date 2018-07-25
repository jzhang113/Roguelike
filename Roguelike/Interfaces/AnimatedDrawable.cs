using RLNET;
using Roguelike.Core;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Interfaces
{
    [Serializable]
    class AnimatedDrawable : Drawable
    {
        private readonly RLColor _accentColor;
        private readonly double _alpha;

        public AnimatedDrawable(RLColor color, char symbol, RLColor accentColor, double luminosity)
            : base(color, symbol, false)
        {
            _accentColor = accentColor;
            _alpha = luminosity;
        }

        public AnimatedDrawable(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            float r = (float)info.GetValue($"{nameof(_accentColor)}.r", typeof(float));
            float g = (float)info.GetValue($"{nameof(_accentColor)}.g", typeof(float));
            float b = (float)info.GetValue($"{nameof(_accentColor)}.b", typeof(float));
            _accentColor = new RLColor(r, g, b);
            _alpha = info.GetDouble(nameof(_alpha));
        }

        public override void Draw(RLConsole console, Tile tile)
        {
            if (!tile.IsExplored)
                return;

            if (!Activated)
                return;

            int destX = X - Camera.X;
            int destY = Y - Camera.Y;

            RLColor newColor = RLColor.Blend(Color, _accentColor,
                (float) (Game.World.Random.NextDouble() * _alpha));
            if (tile.IsVisible)
            {
                console.Set(destX, destY, newColor, null, Symbol);
            }
            else
            {
                //console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
                console.Set(destX, destY, newColor, null, Symbol);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue($"{nameof(_accentColor)}.r", _accentColor.r);
            info.AddValue($"{nameof(_accentColor)}.g", _accentColor.g);
            info.AddValue($"{nameof(_accentColor)}.b", _accentColor.b);
            info.AddValue(nameof(_alpha), _alpha);
        }
    }
}
