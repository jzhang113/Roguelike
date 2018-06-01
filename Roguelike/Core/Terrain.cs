using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Terrain
    {
        public bool IsExplored { get; set; }
        public bool IsWall { get; set; }
        public bool IsOccupied { get => Unit != null; }
        public bool IsWalkable { get => !IsWall && !IsOccupied; }

        public int MoveCost { get; set; }
        public Actor Unit { get; set; }
        public InventoryHandler ItemStack { get; set; }

        public int X { get; }
        public int Y { get; }

        [field: NonSerialized]
        private (int X, int Y) _position;

        public (int X, int Y) Position { get => _position; }

        public Terrain(bool wall, int moveCost, int x, int y)
        {
            IsWall = wall;
            MoveCost = moveCost;
            _position = (X: x, Y: y);
        }
    }
}