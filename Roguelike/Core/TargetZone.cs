using Roguelike.Actors;
using System.Collections.Generic;

namespace Roguelike.Core
{
    struct TargetZone
    {
        public TargetShape Shape { get; }
        public int Range { get; }
        public bool Aimed { get; }
        public (int X, int Y)? Target { get; }

        public TargetZone(TargetShape shape, (int X, int Y)? target = null, int range = 1)
        {
            Shape = shape;
            Range = range;
            Target = target;

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

        public IEnumerable<Terrain> GetTilesInRange(Actor current, (int X, int Y)? target = null)
        {
            int X = 0, Y = 0;
            if (Aimed)
            {
                if (Target != null)
                    (X, Y) = Target.Value;
                else if (target != null)
                    (X, Y) = target.Value;
                else
                    throw new System.ArgumentException("aimed target destination not supplied");
            }

            ICollection<Terrain> inRange = new List<Terrain>();

            switch (Shape)
            {
                case TargetShape.Self:
                    inRange.Add(Game.Map.Field[current.X, current.Y]);
                    return inRange;
                case TargetShape.Area:
                    foreach (Terrain cell in Game.Map.Field)
                    {
                        (X, Y) = cell.Position;
                        int distance = Field.Distance2(current.X, current.Y, X, Y);

                        if (distance > 0 && distance <= Range * Range)
                            inRange.Add(cell);
                    }
                    return inRange;
                case TargetShape.Range:
                    inRange.Add(Game.Map.Field[X, Y]);
                    return inRange;
                case TargetShape.Ray:
                    return Game.Map.StraightLinePath(current.X, current.Y, X, Y);
                case TargetShape.Directional:
                    int dx = current.X - X;
                    int dy = current.Y - Y;
                    int sx = dx / System.Math.Abs(dx);
                    int sy = dy / System.Math.Abs(dy);
                    int limit = System.Math.Abs(dx);

                    for (int i = 0; i < limit; i++)
                        inRange.Add(Game.Map.Field[X + i * sx, Y + i * sy]);

                    return inRange;
                default:
                    throw new System.ArgumentException("unknown skill shape");
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
