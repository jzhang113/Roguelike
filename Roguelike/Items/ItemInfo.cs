using Roguelike.Items;
using System;

namespace Roguelike.Items
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

        public void Add(int count)
        {
            Count += count;
        }
       
        public void Remove(int count)
        {
            System.Diagnostics.Debug.Assert(Count >= count);
            Count -= count;

            if (Count < 0)
                Count = 0;
        }

        public bool Contains(Item item)
        {
            return Item.Equals(item);
        }

        public bool Equals(ItemInfo other)
        {
            return Item.Equals(other.Item);
        }
    }
}
