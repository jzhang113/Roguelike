using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    internal class MoveAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => 0;
        public int EnergyCost => Data.Constants.FULL_TURN;
        public IAnimation Animation => null;

        public MoveAction(TargetZone targetZone)
        {
            Area = targetZone;
        }

        public void Activate(ISchedulable source, Tile target)
        {
            if (source is Actor actor && target.IsWalkable)
            {
                Game.Map.SetActorPosition(actor, target.X, target.Y);
            }
        }
    }
}
