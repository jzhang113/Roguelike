using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Actions
{
    internal class HookAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => Data.Constants.HALF_TURN;
        public int EnergyCost => Data.Constants.FULL_TURN;
        public IAnimation Animation { get; private set; }

        public HookAction(int range)
        {
            Area = new TargetZone(TargetShape.Range, range);
        }

        public void Activate(ISchedulable source, Tile target)
        {
            if (!(source is Actor sourceActor))
                return;

            // Relies on GetTilesInRange being called in the targetting phase.
            // Also requires the hook to be considered a projectile so that the trail is saved.
            IEnumerable<Tile> path = Area.Trail;
            List<Tile> collisionPath = new List<Tile>();

            // Walk along the path, stopping when something is hit and save the collision tile.
            Tile collisionTile = null;
            foreach (Tile tile in path)
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

            LayerInfo currentLayer = Game.StateHandler.CurrentLayer;
            if (collisionTile != null)
            {
                if (Game.Map.TryGetActor(collisionTile.X, collisionTile.Y, out Actor actor))
                {
                    // If an Actor is hit, pull the target in.
                    Tile depositTile = collisionPath[0];
                    int prevX = actor.X;
                    int prevY = actor.Y;
                    Game.Map.SetActorPosition(actor, depositTile.X, depositTile.Y);
                    Animation = new HookAnimation(currentLayer, sourceActor, collisionPath, true, actor);
                }
                else
                {
                    // If something else got hit, it must be a wall or door. In either case, pull
                    // the source towards the target.
                    Tile depositTile = collisionPath.Last();
                    int prevX = sourceActor.X;
                    int prevY = sourceActor.Y;
                    Game.Map.SetActorPosition(sourceActor, depositTile.X, depositTile.Y);
                    Animation = new HookAnimation(currentLayer, sourceActor, collisionPath, false);
                }
            }
            else
            {
                // Otherwise don't do anything.
                Animation = new HookAnimation(currentLayer, sourceActor, collisionPath, true);
            }
        }
    }
}
