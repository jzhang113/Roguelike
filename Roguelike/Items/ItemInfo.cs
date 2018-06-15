using Roguelike.Items;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Items
{
    [Serializable]
    public class ItemInfo : IEquatable<ItemInfo>, ISerializable
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

        protected ItemInfo(SerializationInfo info, StreamingContext context)
        {
            Item = (Item)info.GetValue(nameof(Item), typeof(Item));
            Count = info.GetInt32(nameof(Count));
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Item), Item);
            info.AddValue(nameof(Count), Count);
        }
    }
}
