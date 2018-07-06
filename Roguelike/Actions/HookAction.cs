using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Actions
{
    class HookAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed { get; } = Utils.Constants.FULL_TURN;
        public IAnimation Animation { get; private set; } = null;

        public HookAction(int range)
        {
            Area = new TargetZone(Enums.TargetShape.Range, range: range);
        }

        public void Activate(Actor source, Terrain target)
        {
            IEnumerable<Terrain> path = Game.Map.GetStraightLinePath(source.X, source.Y, target.X, target.Y).ToList();
            List<Terrain> collisionPath = new List<Terrain>();

            // Walk along the path, stopping when something is hit and save the collision tile.
            Terrain collisionTile = null;
            foreach (Terrain tile in path)
            {
                if (!tile.IsWalkable)
                {
                    collisionTile = tile;
                    break;
                }

                collisionPath.Add(tile);
            }

            // Don't do anything if we target a blocked square next to us.
            if (collisionPath.Count == 0)
                return;

            if (collisionTile != null)
            {
                if (Game.Map.TryGetActor(collisionTile.X, collisionTile.Y, out Actor actor))
                {
                    // If an Actor is hit, pull the target in.
                    Animation = new HookAnimation(source, collisionPath, true, actor);
                    Animation.Complete += (sender, args) =>
                    {
                        Terrain depositTile = collisionPath.First();
                        Game.Map.SetActorPosition(actor, depositTile.X, depositTile.Y);
                    };
                }
                else if (Game.Map.Field[collisionTile.X, collisionTile.Y].IsWall)
                {
                    // If a wall is hit, pull the source to the wall.
                    Animation = new HookAnimation(source, collisionPath, false);
                    Animation.Complete += (sender, arg) =>
                    {
                        Terrain depositTile = collisionPath.Last();
                        Game.Map.SetActorPosition(source, depositTile.X, depositTile.Y);
                    };
                }
            }
            else
            {
                // Otherwise don't do anything.
                Animation = new HookAnimation(source, collisionPath, true);
            }
        }
    }
}
