using System;

namespace Roguelike.Core
{
    public readonly struct WeightedPoint
    {
        public int X { get; }
        public int Y { get; }
        public float Weight { get; }

        public WeightedPoint(int x, int y)
        {
            X = x;
            Y = y;
            Weight = 0;
        }

        public WeightedPoint(int x, int y, float weight)
        {
            X = x;
            Y = y;
            Weight = weight;
        }

        #region equality
        public override bool Equals(object obj)
        {
            if (!(obj is WeightedPoint))
            {
                return false;
            }

            WeightedPoint point = (WeightedPoint) obj;
            return X == point.X &&
                   Y == point.Y &&
                   Math.Abs(Weight - point.Weight) < 0.001;
        }

        public override int GetHashCode()
        {
            int hashCode = -1479892828;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Weight.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(WeightedPoint w1, WeightedPoint w2) => w1.Equals(w2);
        public static bool operator !=(WeightedPoint w1, WeightedPoint w2) => !w1.Equals(w2);
        #endregion
    }
}
