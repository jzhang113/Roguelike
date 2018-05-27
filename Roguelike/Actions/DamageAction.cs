using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Actions
{
    class DamageAction : IAction
    {
        public int Power { get; }
        public TargetZone Area { get; }
        public int Speed { get; }

        public DamageAction(int power, TargetZone targetZone)
        {
            Power = power;
            Area = targetZone;
            Speed = Utils.Constants.FULL_TURN;
        }

        // Deals tamage to the target.
        public void Activate(Actor source, Terrain target)
        {
            Actor targetUnit = target.Unit;

            if (targetUnit != null)
            {
                int damage = targetUnit.TakeDamage(Power);

                if (targetUnit.IsDead)
                    targetUnit.State = Enums.ActorState.Dead;

                Game.MessageHandler.AddMessage($"{source.Name} hits {targetUnit.Name} for {damage} damage");
            }
        }
    }
}
