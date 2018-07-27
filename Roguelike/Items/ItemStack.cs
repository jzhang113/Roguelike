using Roguelike.Data;
using Roguelike.Utils;
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
        internal bool Collapsed { get; set; }
        public int TypeCount => _itemStack.Count;

        private readonly IDictionary<Item, int> _itemStack;

        public bool IsEmpty() => _itemStack.Count == 0;

        public ItemStack(Item item, int count)
        {
            Name = item.Name;
            Count = count;
            Collapsed = true;

            _itemStack = new Dictionary<Item, int>
            {
                { item, count }
            };
        }

        public bool Contains(Item item, out Item key)
        {
            foreach (Item itemKey in _itemStack.Keys)
            {
                if (itemKey.SimilarTo(item))
                {
                    key = itemKey;
                    return true;
                }
            }

            key = null;
            return false;
        }

        private bool Similar(Item item, out Item key)
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

            if (Similar(item, out Item key))
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

        // Wooden items burn, potions explode, fun
        public void SetFire()
        {
            foreach (KeyValuePair<Item, int> kvp in _itemStack.ToList())
            {
                Item item = kvp.Key;
                int amount = kvp.Value;

                // Skip if it's already burning.
                if (item.Burning)
                    continue;

                // Use a binomial distribution to determine the number of items that burn.
                MaterialProperty material = item.Parameters.Material.ToProperty();
                int burningCount = Game.Random.NextBinomial(
                    amount,
                    material.Flammability.ToIgniteChance());

                // Remove burning items and re-add them as a new type.
                ItemCount burningStack = Split(item, burningCount);
                burningStack.Item.Burning = true;
                //_itemStack.Add(burningStack.Item, burningStack.Count);
            }
        }

        public IEnumerator<ItemCount> GetEnumerator()
        {
            return _itemStack.Select(
                kvp => new ItemCount { Item = kvp.Key, Count = kvp.Value }).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (Count == 1)
                return $"a {Name}";
            else if (TypeCount == 1)
                return $"{Count} {Name.Pluralize()}";
            else
                return $"{Count} {Name.Pluralize()}*";
        }
    }
}