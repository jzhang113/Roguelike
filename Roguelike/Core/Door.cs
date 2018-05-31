using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class Door : Actor
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
