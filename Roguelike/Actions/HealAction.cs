using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Actions
{
    class HealAction : IAction
    {
        public int Power { get; }
        public TargetZone Area { get; }

        public HealAction(int power, TargetZone targetZone)
        {
            Power = power;
            Area = targetZone;
        }

        // Heals the target by amount up to its maximum health.
        public void Activate(Actor source, Terrain target)
        {
            Actor targetUnit = target.Unit;

            if (target != null)
            {
                int healing = targetUnit.TakeHealing(Power);

                Game.MessageHandler.AddMessage(string.Format("{0} healed {1} by {2} damage", source.Name, targetUnit.Name, healing), Systems.OptionHandler.MessageLevel.Normal);
            }
        }
    }
}