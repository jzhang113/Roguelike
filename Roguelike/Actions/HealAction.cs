using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    class HealAction : IAction
    {
        public int Power { get; }
        public TargetZone Area { get; }
        public int Speed { get; } = Utils.Constants.FULL_TURN;
        public IAnimation Animation { get; } = null;

        public HealAction(int power, TargetZone targetZone)
        {
            Power = power;
            Area = targetZone;
        }

        // Heals the target by amount up to its maximum health.
        public void Activate(Actor source, Terrain target)
        {
            if (target == null)
                return;

            if (!Game.Map.TryGetActor(target.X, target.Y, out Actor targetUnit))
                return;

            int healing = targetUnit.TakeHealing(Power);
            Game.MessageHandler.AddMessage($"{source.Name} healed {targetUnit.Name} by {healing} damage");
        }
    }
}