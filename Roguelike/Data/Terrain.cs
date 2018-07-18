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

    static class TerrainExtensions
    {
        private static readonly IDictionary<TerrainType, TerrainProperty> _terrainList;

        static TerrainExtensions()
        {
            _terrainList = Program.LoadData<IDictionary<TerrainType, TerrainProperty>>("terrain");

            TerrainProperty wall = _terrainList[TerrainType.Wall];
            wall.Color = Swatch.SecondaryLighter;
            wall.Symbol = '#';
            _terrainList[TerrainType.Wall] = wall;

            TerrainProperty grass = _terrainList[TerrainType.Grass];
            grass.Color = Swatch.DbGrass;
            grass.Symbol = '"';
            _terrainList[TerrainType.Grass] = grass;

            TerrainProperty stone = _terrainList[TerrainType.Stone];
            stone.Color = Swatch.DbStone;
            stone.Symbol = '.';
            _terrainList[TerrainType.Stone] = stone;

            TerrainProperty ice = _terrainList[TerrainType.Ice];
            ice.Color = Swatch.DbWater;
            ice.Symbol = '.';
            _terrainList[TerrainType.Ice] = ice;

            TerrainProperty water = _terrainList[TerrainType.Water];
            water.Color = Swatch.DbDeepWater;
            water.Symbol = '\x00f8';
            _terrainList[TerrainType.Water] = water;
        }

        public static TerrainProperty ToProperty(this TerrainType type) => _terrainList[type];
    }
}