using RLNET;
using Roguelike.Core;
using Roguelike.Items;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    public class InventoryHandler
    {
        public IList<ItemInfo> Inventory { get; }

        public InventoryHandler()
        {
            Inventory = new List<ItemInfo>();
        }

        // Increments the item stack if it already exists and adds the item to inventory otherwise.
        public void Add(Item item)
        {
            bool found = false;
            foreach (ItemInfo info in Inventory)
            {
                if (info.Contains(item))
                {
                    found = true;
                    info.Add();
                    break;
                }
            }

            if (!found)
                Inventory.Add(new ItemInfo(item));
        }

        // Decrements the item stack if there are multiple items or removes the item if there is
        // only one.
        public void Remove(Item item)
        {
            bool found = false;
            foreach (ItemInfo info in Inventory)
            {
                if (info.Contains(item))
                {
                    found = true;
                    info.Remove();

                    // We shouldn't be negative here...
                    System.Diagnostics.Debug.Assert(info.Count >= 0);
                    if (info.Count == 0)
                        Inventory.Remove(info);

                    break;
                }
            }

            if (!found)
            {
                System.Diagnostics.Debug.Assert(false, "Cannot remove non-existant item from inventory");
                Game.MessageHandler.AddMessage("Stop that.");
            }
        }
    
        public bool HasKey(char key)
        {
            if (key < 'a' || key > 'z')
                return false;
            
            return key - 'a' < Inventory.Count;
        }

        public Item GetItem(char key)
        {
            System.Diagnostics.Debug.Assert(HasKey(key));
            return Inventory[key - 'a'].Item;
        }

        public void Draw(RLConsole console)
        {
            int line = 1;
            char letter = 'a';

            foreach (ItemInfo info in Inventory)
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
