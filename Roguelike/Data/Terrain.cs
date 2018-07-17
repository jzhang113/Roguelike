using RLNET;
using Roguelike.Core;
using System.Collections.Generic;

namespace Roguelike.Data
{
    public enum TerrainType
    {
        Wall,
        Grass,
        Stone,
        Ice,
        Water
    }

    struct TerrainProperty
    {
        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public double MoveCost { get; set; }
        public Flammability Flammability { get; set; }
    }

    static class Terrain
    {
        public static IDictionary<TerrainType, TerrainProperty> TerrainList;

        static Terrain()
        {
            TerrainList = Program.LoadData<IDictionary<TerrainType, TerrainProperty>>("terrain");

            TerrainProperty wall = TerrainList[TerrainType.Wall];
            wall.Color = Swatch.SecondaryLighter;
            wall.Symbol = '#';

            TerrainProperty grass = TerrainList[TerrainType.Grass];
            grass.Color = Swatch.DbGrass;
            grass.Symbol = '"';

            TerrainProperty stone = TerrainList[TerrainType.Stone];
            stone.Color = Swatch.DbStone;
            stone.Symbol = '.';

            TerrainProperty ice = TerrainList[TerrainType.Ice];
            ice.Color = Swatch.DbWater;
            ice.Symbol = '.';

            TerrainProperty water = TerrainList[TerrainType.Water];
            water.Color = Swatch.DbDeepWater;
            water.Symbol = '~';
        }
    }
}