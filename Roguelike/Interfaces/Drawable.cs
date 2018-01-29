using RLNET;
using Roguelike.Core;
using RogueSharp;

namespace Roguelike.Interfaces
{
    abstract class Drawable
    {
        public abstract RLColor Color { get; set; }
        public abstract char Symbol { get; set; }
        public abstract int X { get; set; }
        public abstract int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
                return;

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                //console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
        }
    }
}
