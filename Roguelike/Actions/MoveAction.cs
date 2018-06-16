using Roguelike.Actors;
using Roguelike.Core;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    class MoveAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed { get; }

        public MoveAction(TargetZone targetZone)
        {
            Area = targetZone;
            Speed = Utils.Constants.FULL_TURN;
        }

        public void Activate(Actor source, Terrain target)
        {
            if (target.IsWalkable)
            {
                Game.Map.SetActorPosition(source, target.X, target.Y);
            }
        }
    }
}
