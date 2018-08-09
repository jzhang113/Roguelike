using Roguelike.Interfaces;
using Roguelike.World;

namespace Roguelike.Core
{
    public class Exit
    {
        public LevelId Destination { get; }
        public Drawable DrawingComponent { get; }

        public int X { get; set; }
        public int Y { get; set; }

        public Exit()
        {
        }

        public Exit(LevelId destination, char symbol)
        {
            Destination = destination;
            DrawingComponent = new Drawable(Colors.Exit, symbol, true);
        }
    }
}
