using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Skills
{
    class DamageAction : IAction
    {
        public int Power { get; }

        public DamageAction(int power)
        {
            Power = power;
        }

        // Deals tamage to the target.
        public void Activate(Terrain target)
        {
            Actor targetUnit = target.Unit;

            if (targetUnit != null)
            {
                int damage = targetUnit.TakeDamage(Power);

                if (targetUnit.IsDead)
                    targetUnit.State = ActorState.Dead;

                Game.MessageHandler.AddMessage(string.Format("{0} takes {1} damage", targetUnit.Name, damage), Systems.OptionHandler.MessageLevel.Normal);
            }
        }
    }
}
