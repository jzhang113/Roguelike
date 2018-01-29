using Roguelike.Core;

namespace Roguelike.Actors
{
    class Skeleton : Actor
    {
        public Skeleton() : base()
        {
            Awareness = 10;
            Name = "Skeleton";
            Color = Colors.TextHeading;
            Symbol = 'S';

            HP = 50;
            SP = 20;
            MP = 20;
        }
    }
}
