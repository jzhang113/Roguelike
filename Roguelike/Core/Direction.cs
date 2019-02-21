using System;

namespace Roguelike.Core
{
    internal static class Direction
    {
        public static readonly Dir N = new Dir(0, -1);
        public static readonly Dir E = new Dir(1, 0);
        public static readonly Dir S = new Dir(0, 1);
        public static readonly Dir W = new Dir(-1, 0);
        public static readonly Dir NE = new Dir(1, -1);
        public static readonly Dir SE = new Dir(1, 1);
        public static readonly Dir SW = new Dir(-1, 1);
        public static readonly Dir NW = new Dir(-1, -1);
        public static readonly Dir Center = new Dir(0, 0);

        public static readonly Dir[] DirectionList = {
            N,
            E,
            S,
            W,
            NE,
            SE,
            SW,
            NW
        };

        public static Dir Right(this Dir dir)
        {
            if (dir == N)
                return NE;
            else if (dir == NE)
                return E;
            else if (dir == E)
                return SE;
            else if (dir == SE)
                return S;
            else if (dir == S)
                return SW;
            else if (dir == SW)
                return W;
            else if (dir == W)
                return NW;
            else if (dir == NW)
                return N;
            else
                return Center;
        }

        public static Dir Left(this Dir dir)
        {
            if (dir == N)
                return NW;
            else if (dir == NE)
                return N;
            else if (dir == E)
                return NE;
            else if (dir == SE)
                return E;
            else if (dir == S)
                return SE;
            else if (dir == SW)
                return S;
            else if (dir == W)
                return SW;
            else if (dir == NW)
                return W;
            else
                return Center;
        }
    }

    [Serializable]
    public readonly struct Dir
    {
        public int X { get; }
        public int Y { get; }

        public Dir(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }

        public override string ToString() => $"({X}, {Y})";

        #region equality
        public override bool Equals(object obj)
        {
            if (!(obj is Dir))
            {
                return false;
            }

            var dir = (Dir)obj;
            return X == dir.X &&
                   Y == dir.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            return hashCode * -1521134295 + Y.GetHashCode();
        }

        public static bool operator ==(in Dir dir1, in Dir dir2) =>
            dir1.X == dir2.X &&
            dir1.Y == dir2.Y;

        public static bool operator !=(in Dir dir1, in Dir dir2) => !(dir1 == dir2);
        #endregion
    }
}
