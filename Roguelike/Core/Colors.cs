using System.Drawing;

namespace Roguelike.Core
{
    internal static class Colors
    {
        public static Color FloorBackground =   Swatch.SecondaryDarkest;
        public static Color Floor =             Swatch.Secondary;

        public static Color WallBackground =    Swatch.AlternateDarkest;
        public static Color Wall =              Swatch.Alternate;

        // Units
        public static Color Player =            Swatch.DbLight;

        // UI
        public static Color Path =              Swatch.Alternate;
        public static Color Target =            Swatch.PrimaryDarker;
        public static Color TargetBackground =  Swatch.Secondary;
        public static Color Cursor =            Swatch.PrimaryDarkest;

        public static Color Text =              Swatch.DbLight;
        public static Color ButtonBackground =  Swatch.Primary;
        public static Color ButtonBorder =      Swatch.Secondary;
        public static Color ButtonHover =       Swatch.Alternate;

        public static Color BorderColor =       Swatch.DbGrass;
        public static Color HighlightColor =    Swatch.DbSun;

        // Terrain
        public static Color Grass =             Swatch.DbGrass;
        public static Color Stone =             Swatch.DbStone;
        public static Color Water =             Swatch.DbWater;

        // Map features
        public static Color Door =              Swatch.DbBrightWood;
        public static Color Exit =              Swatch.Alternate;
        public static Color Hook =              Swatch.DbLight;
        public static Color Fire =              Color.FromArgb(255, 185, 0);
        public static Color FireAccent =        Swatch.DbBlood;
    }
}
