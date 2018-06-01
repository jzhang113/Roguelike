using Roguelike.Items;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class ItemInfo : IEquatable<ItemInfo>
    {
        public Item Item { get; }
        public int Count { get; private set; }

        public ItemInfo(Item item)
        {
            Item = item;
            Count = 1;
        }

        public ItemInfo(Item item, int count)
        {
            Item = item;
            Count = count;
        }

        public void Add()
        {
            Count++;
        }
       
        public void Remove()
        {
            System.Diagnostics.Debug.Assert(Count > 0);
            Count--;
        }

        public bool Contains(Item item)
        {
            return Item == item;
        }

        public bool Equals(ItemInfo other)
        {
            return Item == other.Item;
        }
    }
}
