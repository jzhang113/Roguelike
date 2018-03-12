using Roguelike.Actors;
using System.Collections.Generic;

namespace Roguelike.Core
{
    struct TargetZone
    {
        public TargetShape Shape { get; }
        public int Range { get; }
        public bool Aimed { get; }

        public TargetZone(TargetShape shape, int range = 1)
        {
            Shape = shape;
            Range = range;

            switch (Shape)
            {
                case TargetShape.Self:
                case TargetShape.Area:
                    Aimed = false;
                    break;
                case TargetShape.Range:
                case TargetShape.Ray:
                    Aimed = true;
                    break;
                default:
                    throw new System.ArgumentException("unknown skill shape");
            }
        }

        public IEnumerable<Terrain> GetTilesInRange(Actor current)
        {
            ICollection<Terrain> inRange = new List<Terrain>();

            switch (Shape)
            {
                case TargetShape.Self:
                    inRange.Add(Game.Map.Field[current.X, current.Y]);
                    return inRange;
                case TargetShape.Area:
                    foreach (Terrain cell in Game.Map.Field)
                    {
                        (int x1, int y1) = cell.Position;
                        int distance = Field.Distance2(x1, current.X, y1, current.Y);

                        if (distance > 0 && distance <= Range * Range)
                        {
                            inRange.Add(cell);
                        }
                    }
                    return inRange;
                default:
                    throw new System.ArgumentException("unknown unaimed skill shape");
            }
        }

        public IEnumerable<Terrain> GetTilesInRange(Actor current, (int X, int Y) target)
        {
            ICollection<Terrain> inRange = new List<Terrain>();

            switch (Shape)
            {
                case TargetShape.Range:
                    inRange.Add(Game.Map.Field[target.X, target.Y]);
                    return inRange;
                case TargetShape.Ray:
                    return Game.Map.StraightLinePath(current.X, current.Y, target.X, target.Y);
                default:
                    throw new System.ArgumentException("unknown aimed skill shape");
            }
        }
    }

    enum TargetShape
    {
        Range,
        Area,
        Ray,
        Self
    }
}
