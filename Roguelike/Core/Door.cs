using Roguelike.Actors;
using Roguelike.Commands;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Door : Actor
    {
        public Door() : base(new ActorParameters("Door"), Swatch.DbWood, '+')
        {
        }

        public override ICommand Act()
        {
            if (IsDead)
                TriggerDeath();

            return new WaitCommand(this);
        }
    }
}
