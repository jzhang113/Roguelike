using System;

namespace Roguelike.Core
{
    [Serializable]
    public readonly struct Loc
    {
        public int X { get; }
        public int Y { get; }

        public Loc(int x, int y)
        {
            X = x;
            Y = y;
        }

        #region equality
        public override bool Equals(object obj)
        {
            if (!(obj is Loc))
                return false;

            Loc location = (Loc)obj;
            return X == location.X && Y == location.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(in Loc w1, in Loc w2) =>
            w1.X == w2.X && w1.Y == w2.Y;

        public static bool operator !=(in Loc w1, in Loc w2) => !(w1 == w2);
        #endregion
    }
}
