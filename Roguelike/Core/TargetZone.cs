using Roguelike.Actors;
using System;
using System.Collections.Generic;
using Roguelike.Enums;

namespace Roguelike.Core
{
    [Serializable]
    public class TargetZone
    {
        public TargetShape Shape { get; }
        public double Range { get; }
        public int Radius { get; }
        public bool Projectile { get; }
        public ICollection<Terrain> Trail { get; }

        public TargetZone(TargetShape shape, double range = 1.5, int radius = 0, bool projectile = true)
        {
            Shape = shape;
            Range = range;
            Radius = radius;
            Projectile = projectile;
            Trail = new List<Terrain>();
        }

        public IEnumerable<Terrain> GetTilesInRange(Actor current, int targetX, int targetY)
        {
            ICollection<Terrain> targets = new List<Terrain>();

            switch (Shape)
            {
                case TargetShape.Self:
                    foreach (Terrain tile in Game.Map.GetTilesInRadius(current.X, current.Y, Radius))
                    {
                        if (InRange(current, tile.X, tile.Y))
                            targets.Add(Game.Map.Field[tile.X, tile.Y]);
                    }
                    return targets;
                case TargetShape.Range:
                    int collisionX = targetX;
                    int collisionY = targetY;

                    // for simplicity, assume that the travel path is only 1 tile wide
                    // TODO: trail should be as wide as the Radius
                    if (Projectile)
                    {
                        collisionX = current.X;
                        collisionY = current.Y;
                        Trail.Clear();

                        foreach (Terrain tile in Game.Map.GetStraightLinePath(current.X, current.Y, targetX, targetY))
                        {
                            Trail.Add(tile);
                            collisionX = tile.X;
                            collisionY = tile.Y;

                            if (!tile.IsWalkable)
                                break;
                        }
                    }
                    
                    foreach (Terrain tile in Game.Map.GetTilesInRadius(collisionX, collisionY, Radius))
                    {
                        if (InRange(current, tile.X, tile.Y))
                            targets.Add(Game.Map.Field[tile.X, tile.Y]);
                    }
                    return targets;
                case TargetShape.Ray:
                    IEnumerable<Terrain> path = Game.Map.GetStraightLinePath(current.X, current.Y, targetX, targetY);
                    if (Projectile)
                    {
                        foreach (Terrain tile in path)
                        {
                            // since each step takes us farther away, we can stop checking as soon as one
                            // tile falls out of range
                            if (!InRange(current, tile.X, tile.Y))
                                break;

                            targets.Add(tile);

                            // projectiles stop at the first blocked tile
                            if (!tile.IsWalkable)
                                break;
                        }

                        return targets;
                    }
                    else
                    {
                        return path;
                    }
                case TargetShape.Directional:
                    WeightedPoint dir = Utils.Distance.GetOctant(targetX, targetY, current.X, current.Y);
                    int limit = Math.Max(Math.Abs(targetX - current.X), Math.Abs(targetY - current.Y));

                    for (int i = 1; i <= limit; i++)
                    {
                        int x = current.X + i * dir.X;
                        int y = current.Y + i * dir.Y;

                        // since each step takes us farther away, we can stop checking as soon as one
                        // tile falls out of range
                        if (!InRange(current, x, y))
                            break;

                        Terrain tile = Game.Map.Field[x, y];
                        targets.Add(tile);

                        // projectiles stop at the first blocked tile
                        if (Projectile && !tile.IsWalkable)
                            break;
                    }
                    return targets;
                default:
                    throw new ArgumentException("unknown skill shape");
            }
        }

        private bool InRange(Actor actor, int x, int y)
        {
            int distance = Utils.Distance.EuclideanDistanceSquared(actor.X, actor.Y, x, y);
            return distance > 0 && distance <= Range * Range;
        }
    }
}
