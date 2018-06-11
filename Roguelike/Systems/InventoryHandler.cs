using RLNET;
using Roguelike.Core;
using Roguelike.Items;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    // Handles all stacks of items in the game, such as the player inventory and piles of loot.
    [Serializable]
    public class InventoryHandler : ICollection<ItemInfo>
    {
        private IList<ItemInfo> _inventory;

        public int Count => _inventory.Count;
        public bool IsReadOnly => false;

        // Tells if the current inventory is empty or not.
        public bool IsEmpty() => _inventory.Count == 0;

        public InventoryHandler()
        {
            _inventory = new List<ItemInfo>();
        }

        // Increments the item stack if it already exists or adds the item to inventory.
        public void Add(ItemInfo itemGroup)
        {
            bool found = false;
            foreach (ItemInfo info in _inventory)
            {
                if (info.Equals(itemGroup))
                {
                    found = true;
                    info.Add(itemGroup.Count);
                    break;
                }
            }

            if (!found)
                _inventory.Add(itemGroup);

        }

        // Decrements the item stack if there are multiple items and removes the item if there are
        // no more.
        public bool Remove(ItemInfo itemGroup)
        {
            foreach (ItemInfo info in _inventory)
            {
                if (info.Equals(itemGroup))
                {
                    info.Remove(itemGroup.Count);

                    // We shouldn't be negative here...
                    System.Diagnostics.Debug.Assert(info.Count >= 0);
                    if (info.Count == 0)
                        _inventory.Remove(info);

                    return true;
                }
            }

            System.Diagnostics.Debug.Assert(false, $"Cannot remove non-existant item, {itemGroup.Item.Name}, from inventory");
            Game.MessageHandler.AddMessage($"{itemGroup.Item.Name} not found, can't remove it", Enums.MessageLevel.Verbose);
            return false;
        }

        public void Clear()
        {
            _inventory.Clear();
        }

        public bool Contains(ItemInfo itemGroup)
        {
            foreach (ItemInfo info in _inventory)
            {
                if (info.Equals(itemGroup))
                    return true;
            }

            return false;
        }

        public void CopyTo(ItemInfo[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool HasKey(char key)
        {
            if (key < 'a' || key > 'z')
                return false;

            return key - 'a' < _inventory.Count;
        }

        // Returns the item at position key. Does not remove the item from inventory.
        public ItemInfo GetItem(char key)
        {
            System.Diagnostics.Debug.Assert(HasKey(key));
            return _inventory[key - 'a'];
        }

        public void Draw(RLConsole console)
        {
            int line = 1;
            char letter = 'a';

            foreach (ItemInfo info in _inventory)
            {
                string itemString;

                if (info.Count > 1)
                    itemString = string.Format("{0}) {1} {2}s", letter, info.Count, info.Item.Name);
                else
                    itemString = string.Format("{0}) {1}", letter, info.Item.Name);

                console.Print(1, line, itemString, Colors.TextHeading);
                line++;
                letter++;
            }
        }

        public IEnumerator<ItemInfo> GetEnumerator()
        {
            return _inventory.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
