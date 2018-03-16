using Roguelike.Actors;
using System.Collections.Generic;

namespace Roguelike.Core
{
    struct TargetZone
    {
        public TargetShape Shape { get; }
        public int Range { get; }
        public bool Aimed { get; }
        public bool Continued { get; }

        public TargetZone(TargetShape shape, int range = 1, bool continued = false)
        {
            Shape = shape;
            Range = range;
            Continued = continued;

            switch (Shape)
            {
                case TargetShape.Area:
                case TargetShape.Self:
                    Aimed = false;
                    break;
                case TargetShape.Directional:
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
                case TargetShape.Directional:
                    int dx = current.X - target.X;
                    int dy = current.Y - target.Y;
                    int sx = dx / System.Math.Abs(dx);
                    int sy = dy / System.Math.Abs(dy);
                    int limit = System.Math.Abs(dx);

                    for (int i = 0; i < limit; i++)
                        inRange.Add(Game.Map.Field[target.X + i * sx, target.Y + i * sy]);

                    return inRange;
                default:
                    throw new System.ArgumentException("unknown aimed skill shape");
            }
        }
    }

    enum TargetShape
    {
        Area,
        Directional,
        Range,
        Ray,
        Self
    }
}
