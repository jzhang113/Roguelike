using RLNET;
using Roguelike.Core;
using Roguelike.Items;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    // Handles all stacks of items in the game, such as the player inventory and piles of loot.
    public class InventoryHandler
    {
        private IList<ItemInfo> _inventory;

        public InventoryHandler()
        {
            _inventory = new List<ItemInfo>();
        }

        // Increments the item stack if it already exists and adds the item to inventory otherwise.
        public void Add(Item item)
        {
            bool found = false;
            foreach (ItemInfo info in _inventory)
            {
                if (info.Contains(item))
                {
                    found = true;
                    info.Add();
                    break;
                }
            }

            if (!found)
                _inventory.Add(new ItemInfo(item));
        
        }

        // Decrements the item stack if there are multiple items or removes the item if there is
        // only one.
        public void Remove(Item item)
        {
            bool found = false;
            foreach (ItemInfo info in _inventory)
            {
                if (info.Contains(item))
                {
                    found = true;
                    info.Remove();

                    // We shouldn't be negative here...
                    System.Diagnostics.Debug.Assert(info.Count >= 0);
                    if (info.Count == 0)
                        _inventory.Remove(info);

                    break;
                }
            }

            if (!found)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot remove non-existant item from inventory");
                Game.MessageHandler.AddMessage("Stop that.", OptionHandler.MessageLevel.Verbose);
            }
        }
    
        public bool HasKey(char key)
        {
            if (key < 'a' || key > 'z')
                return false;
            
            return key - 'a' < _inventory.Count;
        }

        // Returns the item at position key. Does not remove the item from inventory.
        public Item GetItem(char key)
        {
            System.Diagnostics.Debug.Assert(HasKey(key));
            return _inventory[key - 'a'].Item;
        }

        // Tells if the current inventory is empty or not.
        public bool IsEmpty() => _inventory.Count == 0;

        // Returns the number of distinct items in the current inventory.
        public int Size() => _inventory.Count;

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
    }
}
