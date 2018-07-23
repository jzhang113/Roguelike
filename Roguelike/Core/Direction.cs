using System;

namespace Roguelike.Core
{
    public enum Direction
    {
        N,
        E,
        S,
        W,
        NE,
        SE,
        SW,
        NW,
        Center
    }

    static class DirectionExtensions
    {
        public static readonly Direction[] DirectionList = new Direction[]
        {
            Direction.N,
            Direction.E,
            Direction.S,
            Direction.W,
            Direction.NE,
            Direction.SE,
            Direction.SW,
            Direction.NW
        };

        public static int GetX(this Direction direction)
        {
            switch (direction)
            {
                case Direction.NW:
                case Direction.W:
                case Direction.SW:
                    return -1;
                case Direction.N:
                case Direction.Center:
                case Direction.S:
                    return 0;
                case Direction.NE:
                case Direction.E:
                case Direction.SE:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException("unknown direction");
            }
        }

        public static int GetY(this Direction direction)
        {
            switch (direction)
            {
                case Direction.NE:
                case Direction.N:
                case Direction.NW:
                    return -1;
                case Direction.E:
                case Direction.Center:
                case Direction.W:
                    return 0;
                case Direction.SE:
                case Direction.S:
                case Direction.SW:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException("unknown direction");
            }
        }
    }
}
