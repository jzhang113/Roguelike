using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.State;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Actions
{
    class HookAction : IAction
    {
        public int Speed { get; }
        public TargetZone Area { get; }

        public HookAction(int range)
        {
            Speed = Utils.Constants.FULL_TURN;
            Area = new TargetZone(Enums.TargetShape.Range, range: range);
        }

        public void Activate(Actor source, Terrain target)
        {
            var path = Game.Map.GetStraightLinePath(source.X, source.Y, target.X, target.Y).ToList();
            var collisionPath = new List<Terrain>();

            foreach (Terrain tile in path)
            {
                if (tile.IsWalkable)
                    collisionPath.Add(tile);
                else
                    break;
            }

            // TODO: Move source or target as appropriate
        }
    }
}
