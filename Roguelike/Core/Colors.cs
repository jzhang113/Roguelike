using RLNET;

namespace Roguelike.Core
{
    static class Colors
    {
        public static RLColor FloorBackground = Swatch.SecondaryDarkest;
        public static RLColor Floor = Swatch.Secondary;

        public static RLColor WallBackground = Swatch.AlternateDarkest;
        public static RLColor Wall = Swatch.Alternate;

        // Units
        public static RLColor Player = Swatch.DbLight;

        // UI
        public static RLColor Path = Swatch.Alternate;
        public static RLColor Target = Swatch.Primary;
        public static RLColor TargetBackground = Swatch.Secondary;
        public static RLColor Cursor = Swatch.PrimaryDarker;

        public static RLColor Text = Swatch.DbLight;
        public static RLColor ButtonBackground = Swatch.Primary;
        public static RLColor ButtonBorder = Swatch.Secondary;
        public static RLColor ButtonHover = Swatch.Alternate;

        // Terrain
        public static RLColor Grass = Swatch.DbGrass;
        public static RLColor Stone = Swatch.DbStone;
        public static RLColor Water = Swatch.DbWater;

        // Map features
        public static RLColor Door = Swatch.DbBrightWood;
        public static RLColor Exit = Swatch.Alternate;
        public static RLColor Hook = Swatch.DbLight;
        public static RLColor Fire = new RLColor(255, 185, 0);
        public static RLColor FireAccent = Swatch.DbBlood;
    }
}
