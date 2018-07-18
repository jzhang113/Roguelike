﻿using RLNET;

namespace Roguelike.Core
{
    static class Colors
    {
        public static RLColor FloorBackground = RLColor.Black;
        public static RLColor Floor = Swatch.AlternateDarkest;
        public static RLColor FloorBackgroundFov = Swatch.DbDark;
        public static RLColor FloorFov = Swatch.Alternate;

        public static RLColor WallBackground = Swatch.SecondaryDarkest;
        public static RLColor Wall = Swatch.Secondary;
        public static RLColor WallBackgroundFov = Swatch.SecondaryDarker;
        public static RLColor WallFov = Swatch.SecondaryLighter;

        public static RLColor TextHeading = Swatch.DbLight;

        public static RLColor Player = Swatch.DbLight;

        public static RLColor PathColor = Swatch.DbSun;
        public static RLColor TargetColor = Swatch.DbBlood;
        public static RLColor Cursor = RLColor.Red;
    }
}
