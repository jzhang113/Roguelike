﻿using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Actions
{
    class DamageAction : IAction
    {
        public int Power { get; }
        public TargetZone Area { get; }

        public DamageAction(int power, TargetZone targetZone)
        {
            Power = power;
            Area = targetZone;
        }

        // Deals tamage to the target.
        public void Activate(Actor source, Terrain target)
        {
            Actor targetUnit = target.Unit;

            if (targetUnit != null)
            {
                int damage = targetUnit.TakeDamage(Power);

                if (targetUnit.IsDead)
                    targetUnit.State = ActorState.Dead;

                Game.MessageHandler.AddMessage(string.Format("{0} hits {1} for {2} damage", source.Name, targetUnit.Name, damage), Systems.OptionHandler.MessageLevel.Normal);
            }
        }
    }
}
