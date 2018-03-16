using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Actions
{
    class MoveAction : IAction
    {
        public TargetZone Area { get; }

        public MoveAction(TargetZone targetZone)
        {
            Area = targetZone;
        }

        public void Activate(Actor source, Terrain target)
        {
            if (target.IsWalkable)
            {
                var (targetX, targetY) = target.Position;
                Game.Map.SetActorPosition(source, targetX, targetY);
            }
        }
    }
}
