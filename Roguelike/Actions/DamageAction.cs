using Optional;
using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    internal class DamageAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed { get; }
        public int EnergyCost { get; }
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        private readonly int _power;

        public DamageAction(int power, TargetZone targetZone,
            int speed = Data.Constants.HALF_TURN, int energy = Data.Constants.FULL_TURN)
        {
            _power = power;
            Speed = speed;
            EnergyCost = energy;
            Area = targetZone;
        }

        // Deals tamage to the target.
        public void Activate(ISchedulable source, Tile target) =>
            Game.Map.GetActor(target.X, target.Y).MatchSome(targetUnit =>
            {
                int damage = targetUnit.TakeDamage(_power);

                if (targetUnit.IsDead)
                    targetUnit.State = ActorState.Dead;

                Game.MessageHandler.AddMessage(
                    $"{source.Name} hits {targetUnit.Name} for {damage} damage");
            });
    }
}
