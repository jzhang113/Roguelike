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
    public class InventoryHandler : ICollection<ItemCount>
    {
        private readonly IList<ItemStack> _inventory;

        public int Count => _inventory.Count;
        public bool IsReadOnly => false;

        public bool IsEmpty() => _inventory.Count == 0;

        public InventoryHandler()
        {
            _inventory = new List<ItemStack>();
        }

        // Increments the item stack if it already exists or adds the item to inventory.
        public void Add(ItemCount itemCount)
        {
            if (itemCount?.Item == null)
                return;

            bool found = false;
            foreach (ItemStack itemStack in _inventory)
            {
                if (itemStack.Contains(itemCount.Item, out _))
                {
                    found = true;
                    itemStack.Add(itemCount.Item, itemCount.Count);
                    break;
                }
            }

            if (!found)
                _inventory.Add(new ItemStack(itemCount.Item, itemCount.Count));
        }

        // Deregister an Item from the Inventory.
        public bool Remove(ItemCount itemCount)
        {
            if (itemCount?.Item == null)
                return false;

            foreach (ItemStack itemStack in _inventory)
            {
                if (itemStack.Contains(itemCount.Item, out Item key))
                {
                    itemStack.Remove(key);

                    if (itemStack.IsEmpty())
                        _inventory.Remove(itemStack);

                    return true;
                }
            }

            return false;
        }

        // Returns a new ItemCount containing the split amount.
        public ItemCount Split(ItemCount itemCount)
        {
            System.Diagnostics.Debug.Assert(itemCount?.Item != null);

            foreach (ItemStack itemStack in _inventory)
            {
                if (itemStack.Contains(itemCount.Item, out Item key))
                {
                    ItemCount split = itemStack.Split(key, itemCount.Count);

                    if (itemStack.IsEmpty())
                        _inventory.Remove(itemStack);

                    return split;
                }
            }

            Game.MessageHandler.AddMessage($"{itemCount.Item.Name} not found, can't split it", MessageLevel.Verbose);
            System.Diagnostics.Debug.Assert(false, $"Cannot split non-existant item, {itemCount.Item.Name}, from inventory");
            return null;
        }

        public void Clear()
        {
            _inventory.Clear();
        }

        public bool Contains(ItemCount itemCount)
        {
            if (itemCount?.Item == null)
                return false;

            return _inventory.Any(itemStack => itemStack.Contains(itemCount.Item, out _));
        }

        public void CopyTo(ItemCount[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool HasKey(char key)
        {
            if (key < 'a' || key > 'z')
                return false;

            return key - 'a' < _inventory.Count;
        }

        // Attempts to returns the item at position key. Does not remove the item from inventory.
        // Returns false if the item does not exist.
        public bool TryGetKey(char key, out ItemCount item)
        {
            if (!HasKey(key))
            {
                item = null;
                return false;
            }

           item = _inventory[key - 'a'].First();
           return true;
        }

        public bool OpenStack(char key)
        {
            if (!HasKey(key))
                return false;

            ItemStack itemStack = _inventory[key - 'a'];
            if (itemStack.TypeCount == 1)
                return false;

            itemStack.Collapsed = false;
            return true;
        }

        public bool CollapseStack(char key)
        {
            if (!HasKey(key))
                return false;

            ItemStack itemStack = _inventory[key - 'a'];
            if (itemStack.TypeCount == 1)
                return false;

            itemStack.Collapsed = true;
            return true;
        }

        public void SetFire()
        {
            foreach (ItemStack stack in _inventory.ToList())
            {
                stack.SetFire();
                if (stack.IsEmpty())
                    _inventory.Remove(stack);
            }
        }

        public void Draw(RLConsole console)
        {
            int line = 1;
            char letter = 'a';

            foreach (ItemStack itemStack in _inventory)
            {
                console.Print(1, line, $"{letter}) {itemStack}", Colors.TextHeading);
                line++;
                letter++;

                if (!itemStack.Collapsed)
                {
                    char subletter = 'a';
                    foreach (ItemCount itemCount in itemStack)
                    {
                        console.Print(3, line, $"{letter} {subletter} - {itemCount}", Colors.TextHeading);
                        line++;
                        subletter++;
                    }
                }
            }
        }

        public IEnumerator<ItemCount> GetEnumerator()
        {
            return _inventory.SelectMany(itemStack => itemStack).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
