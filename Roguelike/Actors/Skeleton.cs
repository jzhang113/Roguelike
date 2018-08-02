using Roguelike.Core;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    class Skeleton : Actor
    {
        public Skeleton(ActorParameters parameters) : base(parameters, Colors.Text, 'S')
        {
        }
    }
}
