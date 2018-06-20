using RLNET;
using Roguelike.Core;
using Roguelike.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Systems
{
    // Handles all stacks of items in the game, such as the player inventory and piles of loot.
    [Serializable]
    public class InventoryHandler : ICollection<Item>
    {
        private readonly IList<Item> _inventory;

        public int Count => _inventory.Count;
        public bool IsReadOnly => false;

        // Tells if the current inventory is empty or not.
        public bool IsEmpty() => _inventory.Count == 0;

        public InventoryHandler()
        {
            _inventory = new List<Item>();
        }

        // Increments the item stack if it already exists or adds the item to inventory.
        public void Add(Item item)
        {
            if (item == null)
                return;

            bool found = false;
            foreach (Item inventoryItem in _inventory)
            {
                if (inventoryItem.SameAs(item))
                {
                    found = true;
                    inventoryItem.Add(item.Count);
                    break;
                }
            }

            if (!found)
                _inventory.Add(item);
        }

        // Deregister an Item from the Inventory.
        public bool Remove(Item item)
        {
            return _inventory.Remove(item);
        }

        // Permanently remove Items from the world. If the Item is still needed later, use Split.
        public bool Destroy(Item item, int amount)
        {
            System.Diagnostics.Debug.Assert(item != null);

            foreach (Item inventoryItem in _inventory)
            {
                if (inventoryItem.Equals(item))
                {
                    item.Remove(amount);

                    if (item.Count == 0)
                        _inventory.Remove(item);

                    return true;
                }
            }

            Game.MessageHandler.AddMessage($"{item.Name} not found, can't remove it", Enums.MessageLevel.Verbose);
            System.Diagnostics.Debug.Assert(false, $"Cannot remove non-existant item, {item.Name}, from inventory");
            return false;
        }

        // Similar to Remove, but returns a new Item containing the split amount.
        public Item Split(Item item, int amount)
        {
            System.Diagnostics.Debug.Assert(item != null);

            foreach (Item inventoryItem in _inventory)
            {
                if (inventoryItem.Equals(item))
                {
                    Item split = item.Split(amount);

                    if (item.Count == 0)
                        _inventory.Remove(item);

                    return split;
                }
            }

            Game.MessageHandler.AddMessage($"{item.Name} not found, can't split it", Enums.MessageLevel.Verbose);
            System.Diagnostics.Debug.Assert(false, $"Cannot split non-existant item, {item.Name}, from inventory");
            return null;
        }

        public void Clear()
        {
            _inventory.Clear();
        }

        public bool Contains(Item item)
        {
            return _inventory.Any(it => it.Equals(item));
        }

        public void CopyTo(Item[] array, int arrayIndex)
        {
            _inventory.CopyTo(array, arrayIndex);
        }

        public bool HasKey(char key)
        {
            if (key < 'a' || key > 'z')
                return false;

            return key - 'a' < _inventory.Count;
        }

        // Attempts to returns the item at position key. Does not remove the item from inventory.
        // Returns false if the item does not exist.
        public bool TryGetKey(char key, out Item item)
        {
            if (!HasKey(key))
            {
                item = null;
                return false;
            }

            item = _inventory[key - 'a'];
            return true;
        }

        public void Draw(RLConsole console)
        {
            int line = 1;
            char letter = 'a';

            foreach (Item item in _inventory)
            {
                var itemString = item.Count > 1
                    ? $"{letter}) {item.Count} {item.Name}s"
                    : $"{letter}) {item.Name}";

                console.Print(1, line, itemString, Colors.TextHeading);
                line++;
                letter++;
            }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return _inventory.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
