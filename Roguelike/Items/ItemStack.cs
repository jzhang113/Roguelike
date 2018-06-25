using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Items
{
    [Serializable]
    class ItemStack : IEnumerable<ItemCount>
    {
        public string Name { get; }
        public int Count { get; private set; }

        private readonly IDictionary<Item, int> _itemStack;

        public ItemStack(Item item, int count)
        {
            Name = item.Name;
            Count = count;

            _itemStack = new Dictionary<Item, int>
            {
                { item, count }
            };
        }

        public bool Contains(Item item, out Item key)
        {
            foreach (Item itemKey in _itemStack.Keys)
            {
                if (itemKey.SameAs(item))
                {
                    key = itemKey;
                    return true;
                }
            }

            key = null;
            return false;
        }

        public void Add(Item item, int count)
        {
            System.Diagnostics.Debug.Assert(item.Name == Name);

            if (Contains(item, out Item key))
                _itemStack[key] += count;
            else
                _itemStack.Add(item, count);

            Count += count;
        }

        public bool Remove(Item item)
        {
            if (item.Name != Name)
                return false;

            return _itemStack.Remove(item);
        }

        public ItemCount Split(Item item, int count)
        {
            System.Diagnostics.Debug.Assert(item.Name == Name);
            System.Diagnostics.Debug.Assert(Contains(item, out _));

            int stackAmount = _itemStack[item];
            if (count >= stackAmount)
            {
                Count -= stackAmount;
                Remove(item);
                return new ItemCount { Item = item, Count = stackAmount };
            }
            else
            {
                _itemStack[item] -= count;
                Count -= count;
                return new ItemCount { Item = item.DeepClone(), Count = count };
            }
        }

        public bool IsEmpty()
        {
            return _itemStack.Count == 0;
        }

        public IEnumerator<ItemCount> GetEnumerator()
        {
            return _itemStack.Select(kvp => new ItemCount { Item = kvp.Key, Count = kvp.Value }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (Count == 1)
                return $"a {Name}";
            else if (_itemStack.Count == 1)
                return $"{Count} {Name.Pluralize()}";
            else
                return $"{Count} {Name.Pluralize()}*";
        }
    }
}