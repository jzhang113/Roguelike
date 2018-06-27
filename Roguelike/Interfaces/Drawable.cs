using RLNET;
using Roguelike.Core;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Interfaces
{
    [Serializable]
    public class Drawable : ISerializable
    {
        public RLColor Color { get; protected internal set; }
        public char Symbol { get; protected internal set; }

        public int X { get; set; }
        public int Y { get; set; }

        public Drawable()
        {
        }

        protected Drawable(SerializationInfo info, StreamingContext context)
        {
            float r = (float)info.GetValue($"{nameof(Color)}.r", typeof(float));
            float g = (float)info.GetValue($"{nameof(Color)}.g", typeof(float));
            float b = (float)info.GetValue($"{nameof(Color)}.b", typeof(float));
            Color = new RLColor(r, g, b);

            Symbol = (char)info.GetValue(nameof(Symbol), typeof(char));
            X = (int)info.GetValue(nameof(X), typeof(int));
            Y = (int)info.GetValue(nameof(Y), typeof(int));
        }

        public void Draw(RLConsole console, Terrain tile, int destX, int destY)
        {
            if (!tile.IsExplored)
                return;

            if (tile.IsVisible)
            {
                console.Set(destX, destY, Color, null, Symbol);
            }
            else
            {
                //console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
                console.Set(destX, destY, Color, null, Symbol);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(Color)}.r", Color.r);
            info.AddValue($"{nameof(Color)}.g", Color.g);
            info.AddValue($"{nameof(Color)}.b", Color.b);

            info.AddValue(nameof(Symbol), Symbol);
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
        }
    }
}
