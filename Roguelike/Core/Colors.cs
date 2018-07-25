using RLNET;

namespace Roguelike.Core
{
    static class Colors
    {
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.DbDark;

        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.DbDark;

        public static RLColor TextHeading = Swatch.DbLight;

        public static RLColor Player = Swatch.DbLight;

        public static RLColor PathColor = Swatch.DbSun;
        public static RLColor TargetColor = Swatch.DbBlood;
        public static RLColor Cursor = RLColor.Red;
    }
}
