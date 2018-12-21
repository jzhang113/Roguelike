using Roguelike.Data;
using Roguelike.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Items
{
    [Serializable]
    public class ItemGroup : ICollection<Item>
    {
        public string Name { get; }
        public int Count { get; private set; }
        private readonly IList<Item> _group;

        public int TypeCount => _group.Count;

        public bool IsReadOnly => false;

        public bool IsEmpty() => TypeCount == 0;

        public bool HasIndex(int index) => index >= 0 && index < TypeCount;

        public ItemGroup(Item item)
        {
            Name = item.Name;
            Count = item.Count;
            _group = new List<Item> { item };
        }

        public bool Contains(Item item) => _group.Any(it => it.SimilarTo(item));

        public void Add(Item item)
        {
            System.Diagnostics.Debug.Assert(item.Name == Name);

            foreach (Item it in _group)
            {
                if (it.SameAs(item))
                {
                    it.Merge(item);
                    Count += item.Count;
                    return;
                }
            }

            _group.Add(item);
            Count += item.Count;
        }

        public bool Remove(Item item)
        {
            System.Diagnostics.Debug.Assert(item.Name == Name);
            return _group.Remove(item);
        }

        public Item Split(Item item, int count)
        {
            System.Diagnostics.Debug.Assert(item.Name == Name);

            foreach (Item it in _group)
            {
                if (it.SameAs(item))
                {
                    Item split = it.Split(count);
                    Count -= split.Count;

                    if (it.Count == 0)
                        _group.Remove(it);

                    return split;
                }
            }

            return null;
        }

        internal Item GetItem(int index)
        {
            if (!HasIndex(index))
                return null;

            return _group[index];
        }

        // Wooden items burn, potions explode, fun
        public void SetFire()
        {
            foreach (Item item in _group)
            {
                // Skip if it's already burning.
                if (item.Burning)
                    continue;

                // Use a binomial distribution to determine the number of items that burn.
                MaterialProperty material = item.Parameters.Material.ToProperty();
                int burningCount = Game.Random.NextBinomial(
                    item.Count, material.Flammability.ToIgniteChance());

                // Remove burning items and re-add them as a new type.
                Item burnt = item.Split(burningCount);
                burnt.Burning = true;
                //_itemStack.Add(burningStack.Item, burningStack.Count);
            }
        }

        #region overrides
        public void Clear()
        {
            _group.Clear();
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            _group.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return _group.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        public override string ToString()
        {
            return (TypeCount == 1) ? _group[0].ToString() : $"{Count} {Name.Pluralize()}*";
        }
    }
}