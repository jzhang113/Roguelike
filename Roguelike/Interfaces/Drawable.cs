using RLNET;
using Roguelike.Systems;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Interfaces
{
    [Serializable]
    public class Drawable : ISerializable
    {
        public RLColor Color { get; internal protected set; }
        public char Symbol { get; internal protected set; }

        public int X { get; set; }
        public int Y { get; set; }

        protected Drawable(SerializationInfo info, StreamingContext context)
        {
            float r = (float)info.GetValue("Color.r", typeof(float));
            float g = (float)info.GetValue("Color.g", typeof(float));
            float b = (float)info.GetValue("Color.b", typeof(float));
            Color = new RLColor(r, g, b);

            Symbol = (char)info.GetValue(nameof(Symbol), typeof(char));
            X = (int)info.GetValue(nameof(X), typeof(int));
            Y = (int)info.GetValue(nameof(Y), typeof(int));
        }

        public void Draw(RLConsole console, MapHandler map)
        {
            //if (!map.Field[X, Y].IsExplored)
            //    return;

            if (map.Field[X, Y].IsVisible)
            {
                console.Set(X, Y, Color, null, Symbol);
            }
            else
            {
                //console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
                console.Set(X, Y, Color, null, Symbol);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Color.r", Color.r);
            info.AddValue("Color.g", Color.g);
            info.AddValue("Color.b", Color.b);
            info.AddValue(nameof(Symbol), Symbol);
            info.AddValue(nameof(X), X);
            info.AddValue(nameof(Y), Y);
        }
    }
}
