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
        public bool IsVisible { get; internal set; }

        public bool IsOccupied { get => Unit != null; }
        public bool IsWalkable { get => !IsWall && !IsOccupied; }

        public int MoveCost { get; set; }
        public Actor Unit { get; set; }
        public InventoryHandler ItemStack { get; set; }

        public int X { get; }
        public int Y { get; }

        public Terrain(bool wall, int moveCost, int x, int y)
        {
            IsWall = wall;
            MoveCost = moveCost;
            X = x;
            Y = y;
        }
    }
}