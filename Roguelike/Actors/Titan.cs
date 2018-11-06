using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;
using System.Collections.Generic;

namespace Roguelike.Actors
{
    [Serializable]
    internal class Titan : Actor, IEquipped
    {
        public EquipmentHandler Equipment { get; }

        private IList<IAction> _attacks;
        private int _current;

        public Titan(ActorParameters parameters) : base(parameters, Swatch.DbBlood, 'T')
        {
            Equipment = new EquipmentHandler();
            Facing = Direction.SE;
            _attacks = new List<IAction>()
            {
                new DamageAction(50, new TargetZone(TargetShape.Range)),
                new DamageAction(50, new TargetZone(TargetShape.Range)),
                new DamageAction(100, new TargetZone(TargetShape.Self, 2)),
            };
            _current = 0;
        }

        public override ICommand GetAction()
        {
            ICommand command = new ActionCommand(this, _attacks[_current], Game.Map.Field[X + Facing.X, Y + Facing.Y]);

            if (++_current >= _attacks.Count)
                _current = 0;

            return command;
        }
    }
}
