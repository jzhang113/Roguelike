using Roguelike.Actors;
using Roguelike.Enums;
using System;
using System.Collections.Generic;

namespace Roguelike.Core
{
    struct TargetZone
    {
        public TargetShape Shape { get; }
        public float Range { get; }
        public bool Aimed { get; }
        public (int X, int Y)? Target { get; }

        public TargetZone(TargetShape shape, (int X, int Y)? target = null, float range = 1.5f)
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
                    throw new ArgumentException("unknown skill shape");
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
                    throw new ArgumentException("aimed target destination not supplied");
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
                        AddCellInRange(current, X, Y, inRange);
                    }
                    return inRange;
                case TargetShape.Range:
                    AddCellInRange(current, X, Y, inRange);
                    return inRange;
                case TargetShape.Ray:
                    return Game.Map.StraightLinePath(current.X, current.Y, X, Y);
                case TargetShape.Directional:
                    int dx = current.X - X;
                    int dy = current.Y - Y;
                    int sx = (dx == 0) ? 0 : dx / Math.Abs(dx);
                    int sy = (dy == 0) ? 0 : dy / Math.Abs(dy);
                    int limit = Math.Max(Math.Abs(dx), Math.Abs(dy));

                    for (int i = 0; i < limit; i++)
                        AddCellInRange(current, X + i * sx, Y + i * sy, inRange);

                    return inRange;
                default:
                    throw new ArgumentException("unknown skill shape");
            }
        }

        public void AddCellInRange(Actor actor, int x, int y, ICollection<Terrain> tiles)
        {
            int distance = Utils.Distance.EuclideanDistanceSquared(actor.X, actor.Y, x, y);
            if (distance > 0 && distance <= Range * Range)
                tiles.Add(Game.Map.Field[x, y]);
        }
    }
}
