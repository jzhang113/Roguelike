using RLNET;
using Roguelike.Systems;
using System;

namespace Roguelike.Interfaces
{
    [Serializable]
    public abstract class Drawable
    {
        private RLColor _color;

        public RLColor Color { get => _color; internal protected set => _color = value; }
        public char Symbol { get; internal protected set; }

        public abstract int X { get; set; }
        public abstract int Y { get; set; }

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
    }
}
