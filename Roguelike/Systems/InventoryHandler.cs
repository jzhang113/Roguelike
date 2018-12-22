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
    public class InventoryHandler : ICollection<Item>
    {
        private readonly IList<ItemGroup> _inventory;

        public int Count => _inventory.Count;

        public char LastKey => (char)('a' + _inventory.Count - 1);

        public bool IsReadOnly => false;

        public bool IsEmpty() => _inventory.Count == 0;

        public InventoryHandler()
        {
            _inventory = new List<ItemGroup>();
        }

        // Increments the item stack if it already exists or adds the item to inventory.
        public void Add(Item item)
        {
            System.Diagnostics.Debug.Assert(item != null);

            foreach (ItemGroup group in _inventory)
            {
                if (group.Contains(item))
                {
                    group.Add(item);
                    return;
                }
            }

            _inventory.Add(new ItemGroup(item));
        }

        // Deregister an Item from the Inventory.
        public bool Remove(Item item)
        {
            System.Diagnostics.Debug.Assert(item != null);

            foreach (ItemGroup group in _inventory)
            {
                if (group.Contains(item))
                {
                    group.Remove(item);
                    if (group.IsEmpty())
                        _inventory.Remove(group);

                    return true;
                }
            }

            return false;
        }

        // Returns a new ItemCount containing the split amount.
        public Item Split(Item item, int count)
        {
            System.Diagnostics.Debug.Assert(item != null);

            foreach (ItemGroup group in _inventory)
            {
                if (group.Contains(item))
                {
                    Item split = group.Split(item, count);
                    if (group.IsEmpty())
                        _inventory.Remove(group);

                    return split;
                }
            }

            Game.MessageHandler.AddMessage(
                $"{item.Name} not found, can't split it",
                MessageLevel.Verbose);
            System.Diagnostics.Debug.Fail(
                $"Cannot split non-existant item, {item.Name}, from inventory");
            return null;
        }

        public void Clear()
        {
            _inventory.Clear();
        }

        public bool Contains(Item item) => _inventory.Any(group => group.Contains(item));

        public void CopyTo(Item[] array, int arrayIndex)
        {
            _inventory.SelectMany(group => group).ToList().CopyTo(array, arrayIndex);
        }

        public bool HasKey(char key) => key >= 'a' && key <= LastKey;

        // Attempts to returns the item at position key. Does not return a stack of items
        public Item GetItem(char key) => (HasKey(key) && !IsStacked(key)) ? _inventory[key - 'a'].First() : null;

        // Attempts to return the stack at position key
        public ItemGroup GetStack(char key) => (HasKey(key) && IsStacked(key)) ? _inventory[key - 'a'] : null;

        public bool IsStacked(char key)
        {
            System.Diagnostics.Debug.Assert(HasKey(key));

            ItemGroup itemStack = _inventory[key - 'a'];
            return itemStack.TypeCount > 1;
        }

        public void SetFire()
        {
            foreach (ItemGroup stack in _inventory.ToList())
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

            foreach (ItemGroup itemStack in _inventory)
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

            foreach (ItemGroup group in _inventory)
            {
                Terminal.Color(group.Any(selected) ? Colors.HighlightColor : Colors.DimText);
                layer.Print(line, $"{letter} - {group}");

                line++;
                letter++;
            }
        }

        // redraw inventory with opened item stack
        internal void DrawStackSelected(LayerInfo layer, char key, Func<Item, bool> selected)
        {
            System.Diagnostics.Debug.Assert(IsStacked(key));

            int line = 1;
            char letter = 'a';
            char subletter = 'a';
            Terminal.Color(Colors.DimText);
            layer.Clear();

            foreach (ItemGroup itemStack in _inventory)
            {
                layer.Print(line++, $"{letter} - {itemStack}");

                if (letter == key)
                {
                    foreach (Item item in _inventory[key - 'a'])
                    {
                        Terminal.Color(selected(item) ? Colors.HighlightColor : Colors.DimText);
                        layer.Print(line++, $"  {subletter} - {item}");
                        subletter++;
                    }

                    Terminal.Color(Colors.DimText);
                }

                letter++;
            }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return _inventory.SelectMany(group => group).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
