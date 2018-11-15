using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    internal class HealAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => Data.Constants.FULL_TURN;
        public IAnimation Animation => null;

        private readonly int _power;

        public HealAction(int power, TargetZone targetZone)
        {
            _power = power;
            Area = targetZone;
        }

        // Heals the target by amount up to its maximum health.
        public void Activate(ISchedulable source, Tile target)
        {
            if (target == null)
                return;

            if (!Game.Map.TryGetActor(target.X, target.Y, out Actor targetUnit))
                return;

            int healing = targetUnit.TakeHealing(_power);
            Game.MessageHandler.AddMessage(
                $"{source.Name} healed {targetUnit.Name} by {healing} damage");
        }
    }
}