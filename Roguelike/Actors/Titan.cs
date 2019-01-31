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

        private readonly IList<IAction> _attacks;
        private int _current;

        public Titan(ActorParameters parameters) : base(parameters, Swatch.DbBlood, 'T')
        {
            Equipment = new EquipmentHandler();
            Facing = Direction.SE;
            _attacks = new List<IAction>()
            {
                new DamageAction(50, new TargetZone(TargetShape.Range)),
                new DamageAction(50, new TargetZone(TargetShape.Range)),
                new DamageAction(100, new TargetZone(TargetShape.Self, 1, 2), 240, 240),
            };
            _current = 0;
        }

        public override ICommand GetAction()
        {
            if (Game.Map.PlayerMap[X, Y] <= 2)
            {
                // in attack range
                // TODO: better decision of when to use large attacks
                Dir dir = Utils.Distance.GetNearestDirection(Game.Player.X, Game.Player.Y, X, Y);
                IAction action = _attacks[_current];
                IEnumerable<Tile> targets = action.Area.GetTilesInRange(this, X + dir.X, Y + dir.Y);
                ICommand command = new DelayActionCommand(this, action, targets);

                if (++_current >= _attacks.Count)
                    _current = 0;

                return command;
            }
            else if (Game.Map.PlayerMap[X, Y] < Parameters.Awareness)
            {
                // in range, chase
                // TODO: don't chase if not alerted
                WeightedPoint nextMove = Game.Map.MoveTowardsTarget(X, Y, Game.Map.PlayerMap);
                return new MoveCommand(this, nextMove.X, nextMove.Y);
            }
            else
            {
                // out of range, sleep
                return new WaitCommand(this);
            }
        }
    }
}
