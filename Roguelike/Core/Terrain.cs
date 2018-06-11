using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Systems;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Terrain
    {
        public bool IsExplored { get; internal set; }
        public bool IsWall { get; internal set; }
        public bool IsVisible { get; internal set; }

        public bool IsOccupied { get => Unit != null; }
        public bool IsWalkable { get => !IsWall && !IsOccupied; }
        public bool BlocksLight
        {
            get
            {
                if (IsWall)
                    return true;

                if (IsOccupied)
                    return Unit.BlocksLight;

                if (ItemStack == null)
                    return false;

                foreach (ItemInfo itemGroup in ItemStack)
                {
                    if (itemGroup.Item.BlocksLight)
                        return true;
                }

                return false;
            }
        }

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