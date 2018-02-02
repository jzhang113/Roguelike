using RLNET;
using Roguelike.Core;
using Roguelike.Items;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    public class InventoryHandler
    {
        public IDictionary<Item, int> Inventory { get; }

        public InventoryHandler()
        {
            Inventory = new Dictionary<Item, int>();
        }

        public void Add(Item item)
        {
            if (Inventory.ContainsKey(item))
                Inventory[item]++;
            else
                Inventory.Add(item, 1);
        }

        public void Remove(Item item)
        {
            System.Diagnostics.Debug.Assert(Inventory.ContainsKey(item));

            int itemCount = Inventory[item]--;

            if (itemCount == 0)
                Inventory.Remove(item);
        }

        public void Draw(RLConsole console)
        {
            int line = 1;
            char letter = 'a';

            foreach (KeyValuePair<Item, int> pair in Inventory)
            {
                string itemString;

                if (pair.Value > 1)
                    itemString = string.Format("{0}) {1} {2}s", letter, pair.Value, pair.Key.Name);
                else
                    itemString = string.Format("{0}) {1}", letter, pair.Key.Name);

                console.Print(1, line, itemString, Colors.TextHeading);
                line++;
                letter++;
            }
        }
    }
}
