using BearLib;
using Roguelike.Core;
using Roguelike.Data;
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

        public char LastKey => (char)('a' + _inventory.Count - 1);

        public bool IsReadOnly => false;

        public bool IsEmpty() => _inventory.Count == 0;

        public InventoryHandler()
        {
            _inventory = new List<ItemStack>();
        }

        // Increments the item stack if it already exists or adds the item to inventory.
        public void Add(ItemCount item)
        {
            if (item?.Item == null)
                return;

            bool found = false;
            foreach (ItemStack itemStack in _inventory)
            {
                if (itemStack.Contains(item.Item, out _))
                {
                    found = true;
                    itemStack.Add(item.Item, item.Count);
                    break;
                }
            }

            if (!found)
                _inventory.Add(new ItemStack(item.Item, item.Count));
        }

        // Deregister an Item from the Inventory.
        public bool Remove(ItemCount item)
        {
            if (item?.Item == null)
                return false;

            foreach (ItemStack itemStack in _inventory)
            {
                if (itemStack.Contains(item.Item, out Item key))
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

            Game.MessageHandler.AddMessage(
                $"{itemCount.Item.Name} not found, can't split it",
                MessageLevel.Verbose);
            System.Diagnostics.Debug.Fail(
                $"Cannot split non-existant item, {itemCount.Item.Name}, from inventory");
            return null;
        }

        public void Clear()
        {
            _inventory.Clear();
        }

        public bool Contains(ItemCount item)
        {
            if (item?.Item == null)
                return false;

            return _inventory.Any(itemStack => itemStack.Contains(item.Item, out _));
        }

        public void CopyTo(ItemCount[] array, int arrayIndex)
        {
            _inventory.SelectMany(itemStack => itemStack).ToList().CopyTo(array, arrayIndex);
        }

        public bool HasKey(char key) => key >= 'a' && key <= LastKey;

        // Attempts to returns the item at position key. Does not remove the item from inventory.
        // Returns false if the item does not exist.
        public bool TryGetKey(char key, out ItemStack item)
        {
            if (!HasKey(key))
            {
                item = null;
                return false;
            }

            item = _inventory[key - 'a'];
            return true;
        }

        public bool IsStacked(char key)
        {
            System.Diagnostics.Debug.Assert(HasKey(key));

            ItemStack itemStack = _inventory[key - 'a'];
            return itemStack.TypeCount > 1;
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

        public void Draw(LayerInfo layer)
        {
            Game.ShowEquip = false;
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopRightChar = '╗', // 187
                BottomRightChar = '╝', // 188
                TopChar = '═', // 205
                BottomChar = '═',
                RightChar = '║' // 186
            });
            layer.Print(-1, $"{Constants.HEADER_LEFT}[color=white]INVENTORY{Constants.HEADER_SEP}" +
                $"[/color]EQUIPMENT{Constants.HEADER_RIGHT}");

            // draw items
            int line = 1;
            char letter = 'a';
            Terminal.Color(Colors.Text);

            foreach (ItemStack itemStack in _inventory)
            {
                layer.Print(line, $"{letter} - {itemStack}");
                line++;
                letter++;
            }
        }

        // redraw selected items
        internal void DrawSelected(LayerInfo layer, Func<Item, bool> selected)
        {
            int line = 1;
            char letter = 'a';
            Terminal.Color(Colors.HighlightColor);

            foreach (ItemStack itemStack in _inventory)
            {
                foreach (ItemCount itemCount in itemStack)
                {
                    if (selected(itemCount.Item))
                    {
                        layer.Print(line, $"{letter} - {itemStack}");
                        break;
                    }
                }

                line++;
                letter++;
            }
        }

        // redraw inventory with opened item stack
        internal void DrawItemStack(LayerInfo layer, char key)
        {
            System.Diagnostics.Debug.Assert(IsStacked(key));

            int line = 1;
            char letter = 'a';
            char subletter = 'a';
            Terminal.Color(Colors.WallBackground);
            layer.Clear();

            foreach (ItemStack itemStack in _inventory)
            {
                layer.Print(line++, $"{letter} - {itemStack}");

                if (letter == key)
                {
                    Terminal.Color(Colors.HighlightColor);

                    foreach (ItemCount itemCount in _inventory[key - 'a'])
                    {
                        layer.Print(line++, $"  {subletter} - {itemCount}");
                        subletter++;
                    }

                    Terminal.Color(Colors.WallBackground);
                }

                letter++;
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
