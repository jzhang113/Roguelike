using Roguelike.Actors;
using Roguelike.Systems;

namespace Roguelike.Core
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