using Roguelike.Core;
using Roguelike.Interfaces;
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

    internal struct TerrainProperty
    {
        public double MoveCost { get; set; }
        public Flammability Flammability { get; set; }
    }

    internal static class TerrainExtensions
    {
        private static readonly IDictionary<TerrainType, TerrainProperty> _terrainList;
        private static readonly IDictionary<TerrainType, Drawable> _drawableList;

        static TerrainExtensions()
        {
            _terrainList = Program.LoadData<IDictionary<TerrainType, TerrainProperty>>("terrain");

            _drawableList = new Dictionary<TerrainType, Drawable>
            {
                [TerrainType.Wall] = new Drawable(Colors.Wall, '#', true),
                [TerrainType.Grass] = new Drawable(Colors.Grass, '"', true),
                [TerrainType.Stone] = new Drawable(Colors.Stone, '.', true),
                [TerrainType.Ice] = new Drawable(Colors.Water, '.', true),
                [TerrainType.Water] = new Drawable(Colors.Water, '\x00f8', true)
            };
        }

        public static TerrainProperty ToProperty(this TerrainType type) => _terrainList[type];
        public static Drawable ToDrawable(this TerrainType type) => _drawableList[type];
    }
}