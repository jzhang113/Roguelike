using Roguelike.Core;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    class Skeleton : Actor
    {
        public Skeleton()
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
