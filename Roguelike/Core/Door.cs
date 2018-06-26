using Roguelike.Actors;
using Roguelike.Commands;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Door : Actor
    {
        public bool IsOpen { get; set; }

        public Door(bool open = false) : base(new ActorParameters("Door"), Swatch.DbWood, '+')
        {
            IsOpen = open;
        }

        public override ICommand Act()
        {
            if (IsDead)
                TriggerDeath();

            return new WaitCommand(this);
        }
    }
}
