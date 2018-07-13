using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    class MoveAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => Utils.Constants.FULL_TURN;
        public IAnimation Animation => null;

        public MoveAction(TargetZone targetZone)
        {
            Area = targetZone;
        }

        public void Activate(ISchedulable source, Terrain target)
        {
            if (source is Actor actor && target.IsWalkable)
            {
                Game.Map.SetActorPosition(actor, target.X, target.Y);
            }
        }
    }
}
