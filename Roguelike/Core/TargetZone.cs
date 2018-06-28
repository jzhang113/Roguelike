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
        public bool Aimed { get; }
        public (int X, int Y)? Target { get; }

        public bool InputRequired => Aimed && Target == null;

        public TargetZone(TargetShape shape, (int X, int Y)? target = null, double range = 1.5)
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
            int x = 0, y = 0;
            if (Aimed)
            {
                if (Target != null)
                    (x, y) = Target.Value;
                else if (target != null)
                    (x, y) = target.Value;
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
                    foreach (Terrain tile in Game.Map.Field)
                    {
                        AddTilesInRange(current, tile.X, tile.Y, inRange);
                    }
                    return inRange;
                case TargetShape.Range:
                    AddTilesInRange(current, x, y, inRange);
                    return inRange;
                case TargetShape.Ray:
                    return Game.Map.GetStraightLinePath(current.X, current.Y, x, y);
                case TargetShape.Directional:
                    int dx = current.X - x;
                    int dy = current.Y - y;
                    int sx = Math.Sign(dx);
                    int sy = Math.Sign(dy);
                    int limit = Math.Max(Math.Abs(dx), Math.Abs(dy));

                    for (int i = 0; i < limit; i++)
                        AddTilesInRange(current, x + i * sx, y + i * sy, inRange);

                    return inRange;
                default:
                    throw new ArgumentException("unknown skill shape");
            }
        }

        public void AddTilesInRange(Actor actor, int x, int y, ICollection<Terrain> tiles)
        {
            int distance = Utils.Distance.EuclideanDistanceSquared(actor.X, actor.Y, x, y);
            if (distance > 0 && distance <= Range * Range)
                tiles.Add(Game.Map.Field[x, y]);
        }
    }
}
