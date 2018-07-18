using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    class DamageAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => Data.Constants.FULL_TURN;
        public IAnimation Animation => null;

        private readonly int _power;

        public DamageAction(int power, TargetZone targetZone)
        {
            _power = power;
            Area = targetZone;
        }

        // Deals tamage to the target.
        public void Activate(ISchedulable source, Tile target)
        {
            if (target == null)
                return;

            if (!Game.Map.TryGetActor(target.X, target.Y, out Actor targetUnit))
                return;

            int damage = targetUnit.TakeDamage(_power);

            if (targetUnit.IsDead)
                targetUnit.State = ActorState.Dead;

            Game.MessageHandler.AddMessage(
                $"{source.Name} hits {targetUnit.Name} for {damage} damage");
        }
    }
}
