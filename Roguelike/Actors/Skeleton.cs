using Roguelike.Core;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Actors
{
    [Serializable]
    class Skeleton : Actor
    {
        public Skeleton()
        {
            Awareness = 10;
            Name = "Skeleton";
            DrawingComponent.Color = Colors.TextHeading;
            DrawingComponent.Symbol = 'S';

            Hp = 50;
            Sp = 20;
            Mp = 20;
        }
    }
}
