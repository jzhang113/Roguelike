using Optional;
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
        public Option<IAnimation> Animation { get; private set; }

        public HookAction(int range)
        {
            Area = new TargetZone(TargetShape.Range, range);
        }

        public void Activate(ISchedulable source, Loc target)
        {
            if (!(source is Actor sourceActor))
            {
                return;
            }

            // Relies on GetTilesInRange being called in the targetting phase.
            // Also requires the hook to be considered a projectile so that the trail is saved.
            IEnumerable<Loc> path = Area.Trail;
            List<Loc> collisionPath = new List<Loc>();

            // Walk along the path, stopping when something is hit and save the collision tile.
            Option<Loc> collision = Option.None<Loc>();
            foreach (Loc point in path)
            {
                if (!Game.Map.Field[point].IsWalkable)
                {
                    collision = Option.Some(point);
                    break;
                }

                collisionPath.Add(point);
            }

            // Don't do anything if we target a blocked square next to us.
            if (collisionPath.Count == 0)
            {
                return;
            }

            LayerInfo currentLayer = Game.StateHandler.CurrentLayer;

            collision.Match(
                some: pos =>
                {
                    Game.Map.GetActor(pos).Match(
                        some: actor =>
                        {
                            // If an Actor is hit, pull the target in.
                            Loc deposit = collisionPath[0];
                            Game.Map.SetActorPosition(actor, deposit);
                            Animation = Option.Some<IAnimation>(new HookAnimation(currentLayer, sourceActor, collisionPath, true, actor));
                        },
                        none: () =>
                        {
                            // If something else got hit, it must be a wall or door. In either case, pull
                            // the source towards the target.
                            Loc deposit = collisionPath.Last();
                            Game.Map.SetActorPosition(sourceActor, deposit);
                            Animation = Option.Some<IAnimation>(new HookAnimation(currentLayer, sourceActor, collisionPath, false));
                        });
                },
                none: () =>
                {
                    // Otherwise don't do anything.
                    Animation = Option.Some<IAnimation>(new HookAnimation(currentLayer, sourceActor, collisionPath, true));
                });
        }
    }
}
