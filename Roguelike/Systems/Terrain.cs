using Roguelike.Actors;
using Roguelike.Items;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    public struct Terrain
    {
        public bool IsWalkable { get; set; }
        public int MoveCost { get; set; }
        public Actor Unit { get; set; }
        public InventoryHandler ItemStack { get; set; }

        public Terrain(bool walkable, int moveCost)
        {
            IsWalkable = walkable;
            MoveCost = moveCost;
            Unit = null;
            ItemStack = new InventoryHandler();
        }
    }
}