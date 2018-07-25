namespace Roguelike.Core
{
    static class Direction
    {
        public static readonly (int X, int Y) N = (0, -1);
        public static readonly (int X, int Y) E = (1, 0);
        public static readonly (int X, int Y) S = (0, 1);
        public static readonly (int X, int Y) W = (-1, 0);
        public static readonly (int X, int Y) NE = (1, -1);
        public static readonly (int X, int Y) SE = (1, 1);
        public static readonly (int X, int Y) SW = (-1, 1);
        public static readonly (int X, int Y) NW = (-1, -1);
        public static readonly (int X, int Y) Center = (0, 0);

        public static readonly (int X, int Y)[] DirectionList = {
            N,
            E,
            S,
            W,
            NE,
            SE,
            SW,
            NW
        };
    }
}
