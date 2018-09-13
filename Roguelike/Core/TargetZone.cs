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
        public ICollection<Tile> Trail { get; }

        private ICollection<Tile> Targets { get; }

        public TargetZone(TargetShape shape, int range = 1, int radius = 0, bool projectile = true)
        {
            Shape = shape;
            Range = range;
            Radius = radius;
            Projectile = projectile;
            Trail = new List<Tile>();
            Targets = new List<Tile>();
        }

        public IEnumerable<Tile> GetTilesInRange(Actor current, int targetX, int targetY)
        {
            Targets.Clear();

            switch (Shape)
            {
                case TargetShape.Self:
                    foreach (Tile tile in Game.Map.GetTilesInRadius(current.X, current.Y, Radius))
                    {
                        if (InRange(current, tile.X, tile.Y))
                            Targets.Add(Game.Map.Field[tile.X, tile.Y]);
                    }
                    return Targets;
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

                        foreach (Tile tile in
                            Game.Map.GetStraightLinePath(current.X, current.Y, targetX, targetY))
                        {
                            Trail.Add(tile);
                            collisionX = tile.X;
                            collisionY = tile.Y;

                            if (!tile.IsWalkable)
                                break;
                        }
                    }

                    foreach (Tile tile in Game.Map.GetTilesInRadius(collisionX, collisionY, Radius))
                    {
                        // TODO: prevent large radius spells from hitting past walls.
                        Targets.Add(Game.Map.Field[tile.X, tile.Y]);
                    }
                    return Targets;
                case TargetShape.Ray:
                    IEnumerable<Tile> path = Game.Map.GetStraightLinePath(
                        current.X, current.Y, targetX, targetY);
                    if (Projectile)
                    {
                        foreach (Tile tile in path)
                        {
                            // since each step takes us farther away, we can stop checking as soon
                            // as one tile falls out of range
                            if (!InRange(current, tile.X, tile.Y))
                                break;

                            Targets.Add(tile);

                            // projectiles stop at the first blocked tile
                            if (!tile.IsWalkable)
                                break;
                        }

                        return Targets;
                    }
                    else
                    {
                        return path;
                    }
                case TargetShape.Directional:
                    var (dx, dy) = Utils.Distance.GetNearestDirection(targetX, targetY, current.X, current.Y);
                    int limit = Math.Max(Math.Abs(targetX - current.X), Math.Abs(targetY - current.Y));

                    for (int i = 1; i <= limit; i++)
                    {
                        int x = current.X + i * dx;
                        int y = current.Y + i * dy;

                        // since each step takes us farther away, we can stop checking as soon as one
                        // tile falls out of range
                        if (!InRange(current, x, y))
                            break;

                        Tile tile = Game.Map.Field[x, y];
                        Targets.Add(tile);

                        // projectiles stop at the first blocked tile
                        if (Projectile && !tile.IsWalkable)
                            break;
                    }
                    return Targets;
                default:
                    throw new ArgumentException("unknown skill shape");
            }
        }

        private bool InRange(Actor actor, int x, int y)
        {
            // square ranges
            int distance = Math.Max(Math.Abs(actor.X - x), Math.Abs(actor.Y - y));
            return distance <= Range;
        }
    }
}
