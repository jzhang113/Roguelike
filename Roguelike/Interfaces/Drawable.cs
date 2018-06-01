using RLNET;
using RogueSharp;
using System;

namespace Roguelike.Interfaces
{
    [Serializable]
    public abstract class Drawable
    {
        [field: NonSerialized]
        private RLColor _color;

        public RLColor Color { get => _color; internal protected set => _color = value; }
        public char Symbol { get; internal protected set; }

        public abstract int X { get; set; }
        public abstract int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
                return;

            if (map.IsInFov(X, Y))
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
