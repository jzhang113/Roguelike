using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
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
            if (target == null)
                return;

            if (Game.Map.TryGetActor(target.X, target.Y, out Actor targetUnit))
            {
                int damage = targetUnit.TakeDamage(Power);

                if (targetUnit.IsDead)
                    targetUnit.State = Enums.ActorState.Dead;

                Game.MessageHandler.AddMessage($"{source.Name} hits {targetUnit.Name} for {damage} damage");
            }
        }
    }
}
