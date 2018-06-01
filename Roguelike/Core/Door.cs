using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Door : Actor
    {
        public Door()
        {
            Name = "door";
            Color = Swatch.DbBrightWood;
            Symbol = '+';
        }

        public override ICommand Act()
        {
            if (IsDead)
                TriggerDeath();

            return new WaitCommand(this);
        }
    }
}
