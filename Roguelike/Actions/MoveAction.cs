using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    class MoveAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed { get; } = Utils.Constants.FULL_TURN;
        public IAnimation Animation { get; } = null;

        public MoveAction(TargetZone targetZone)
        {
            Area = targetZone;
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
