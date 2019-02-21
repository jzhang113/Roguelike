using Roguelike.Actors;
using System;
using System.Collections.Generic;

namespace Roguelike.Core
{
    public enum TargetShape
    {
        Self,
        Range,
        Ray,
        Directional
    }

    [Serializable]
    public class TargetZone
    {
        public TargetShape Shape { get; }
        public int Range { get; }
        public int Radius { get; }
        public bool Projectile { get; }
        public ICollection<Loc> Trail { get; }

        private ICollection<Loc> Targets { get; }

        public TargetZone(TargetShape shape, int range = 1, int radius = 0, bool projectile = true)
        {
            Shape = shape;
            Range = range;
            Radius = radius;
            Projectile = projectile;
            Trail = new List<Loc>();
            Targets = new List<Loc>();
        }

        public IEnumerable<Loc> GetTilesInRange(Actor current, in Loc target)
        {
            Targets.Clear();

            switch (Shape)
            {
                case TargetShape.Self:
                    foreach (Loc point in Game.Map.GetPointsInRadius(current.Loc, Radius))
                    {
                        if (InRange(current, point))
                            Targets.Add(point);
                    }
                    return Targets;
                case TargetShape.Range:
                    Loc collision = target;

                    // for simplicity, assume that the travel path is only 1 tile wide
                    // TODO: trail should be as wide as the Radius
                    if (Projectile)
                    {
                        collision = current.Loc;
                        Trail.Clear();

                        foreach (Loc point in Game.Map.GetStraightLinePath(current.Loc, target))
                        {
                            Trail.Add(point);
                            collision = point;

                            if (!Game.Map.Field[point.X, point.Y].IsWalkable)
                                break;
                        }
                    }

                    foreach (Loc point in Game.Map.GetPointsInRadius(collision, Radius))
                    {
                        // TODO: prevent large radius spells from hitting past walls.
                        Targets.Add(point);
                    }
                    return Targets;
                case TargetShape.Ray:
                    IEnumerable<Loc> path = Game.Map.GetStraightLinePath(current.Loc, target);
                    if (Projectile)
                    {
                        foreach (Loc point in path)
                        {
                            // since each step takes us farther away, we can stop checking as soon
                            // as one tile falls out of range
                            if (!InRange(current, point))
                                break;

                            Targets.Add(point);

                            // projectiles stop at the first blocked tile
                            if (!Game.Map.Field[point].IsWalkable)
                                break;
                        }

                        return Targets;
                    }
                    else
                    {
                        return path;
                    }
                case TargetShape.Directional:
                    var (dx, dy) = Utils.Distance.GetNearestDirection(target, current.Loc);
                    int limit = Math.Max(Math.Abs(target.X - current.Loc.X), Math.Abs(target.Y - current.Loc.Y));

                    for (int i = 1; i <= limit; i++)
                    {
                        Loc posInDir = current.Loc + (i * dx, i * dy);

                        // since each step takes us farther away, we can stop checking as soon as one
                        // tile falls out of range
                        if (!InRange(current, posInDir))
                            break;

                        Targets.Add(posInDir);

                        // projectiles stop at the first blocked tile
                        if (Projectile && !Game.Map.Field[posInDir].IsWalkable)
                            break;
                    }
                    return Targets;
                default:
                    throw new ArgumentException("unknown skill shape");
            }
        }

        private bool InRange(Actor actor, in Loc target)
        {
            // square ranges
            int distance = Math.Max(Math.Abs(actor.Loc.X - target.X), Math.Abs(actor.Loc.Y - target.Y));
            return distance <= Range;
        }
    }
}
