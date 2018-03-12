using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Skills
{
    internal class HealAction : IAction
    {
        public int Power { get; }

        public HealAction(int power)
        {
            Power = power;
        }

        // Heals the target by amount up to its maximum health.
        public void Activate(Terrain target)
        {
            Actor targetUnit = target.Unit;

            if (target != null)
            {
                int healing = targetUnit.TakeHealing(Power);

                Game.MessageHandler.AddMessage(string.Format("{0} healed {1} damage", targetUnit.Name, healing), Systems.OptionHandler.MessageLevel.Normal);
            }
        }
    }
}