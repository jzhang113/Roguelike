using Roguelike.Actors;
using Roguelike.Commands;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Door : Actor
    {
        public Door()
        {
            Name = "door";
            DrawingComponent.Color = Swatch.DbBrightWood;
            DrawingComponent.Symbol = '+';
        }

        public override ICommand Act()
        {
            if (IsDead)
                TriggerDeath();

            return new WaitCommand(this);
        }
    }
}
