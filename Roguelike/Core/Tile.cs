using Roguelike.Data;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Tile
    {
        public int X { get; }
        public int Y { get; }
        public Drawable DrawingComponent { get; }

        public TerrainType Type
        {
            get => _type;
            set
            {
                _type = value;

                TerrainProperty terrain = value.ToProperty();
                DrawingComponent.Color = terrain.Color;
                DrawingComponent.Symbol = terrain.Symbol;
            }
        }

        public int Fuel { get; internal set; }
        public bool IsExplored { get; internal set; }
        public bool IsVisible { get; internal set; }
        public bool IsOccupied { get; internal set; }
        public bool BlocksLight { get; internal set; }

        public bool IsWall => _type == TerrainType.Wall;
        public bool IsWalkable => !IsWall && !IsOccupied;
        public bool IsLightable => !IsWall && !BlocksLight;

        private TerrainType _type;

        public Tile(int x, int y, TerrainType type)
        {
            X = x;
            Y = y;
            Fuel = 10;

            TerrainProperty terrain = type.ToProperty();
            DrawingComponent = new Drawable
            {
                Color = terrain.Color,
                Symbol = terrain.Symbol,
                X = x,
                Y = y
            };
        }
    }
}