using Roguelike.Actors;
using System.Collections.Generic;

namespace Roguelike.Core
{
    struct TargetZone
    {
        TargetShape Shape { get; }
        int Range { get; }

        public TargetZone(TargetShape shape, int range = 1)
        {
            Shape = shape;
            Range = range;
        }

        public IEnumerable<Terrain> GetTilesInRange(Actor current, Terrain endPoint)
        {
            ICollection<Terrain> inRange = new List<Terrain>();
            //// TODO: replace placeholder with targetting system
            (int xTarget, int yTarget) = endPoint?.Position ?? (0, 0);

            switch (Shape)
            {
                case TargetShape.Range:
                    inRange.Add(endPoint);
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
                case TargetShape.Ray:
                    return Game.Map.StraightLinePath(current.X, current.Y, xTarget, yTarget);
                case TargetShape.Self:
                    inRange.Add(Game.Map.Field[current.X, current.Y]);
                    return inRange;
                default:
                    return inRange;
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
