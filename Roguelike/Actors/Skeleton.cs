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

            HP = 50;
            SP = 20;
            MP = 20;
        }

        protected Skeleton(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
