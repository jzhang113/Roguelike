using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System.Linq;

namespace Roguelike.Commands
{
    class PickupCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 60;

        private InventoryHandler _itemStack;

        public PickupCommand(Actor source, InventoryHandler itemStack)
        {
            Source = source;
            _itemStack = itemStack;
        }

        public RedirectMessage Validate()
        {
            // Trying to pick up an empty tile.
            if (_itemStack == null || _itemStack.IsEmpty())
            {
                Game.MessageHandler.AddMessage("There's nothing to pick up here.");
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            ItemInfo itemGroup;

            switch(_itemStack.Count)
            {
                case 1:
                    itemGroup = _itemStack.First();
                    Source.Inventory.Add(itemGroup);
                    itemGroup.Item.Carrier = Source;

                    Game.Map.RemoveItem(itemGroup);
                    Game.MessageHandler.AddMessage($"You pick up a {itemGroup.Item.Name}.");
                    break;
                default:
                    if (Source is Player)
                    {
                        // HACK: handle pickup menu - this placeholder at least lets you pick up the top item
                        itemGroup = _itemStack.First();
                        Source.Inventory.Add(itemGroup);
                        itemGroup.Item.Carrier = Source;

                        Game.Map.RemoveItem(itemGroup);
                        Game.MessageHandler.AddMessage($"You pick up a {itemGroup.Item.Name}.");
                    }
                    else
                    {
                        // HACK: Monsters will simply grab the top item off of a pile if they try to pick stuff up.
                        itemGroup = _itemStack.First();
                        Source.Inventory.Add(itemGroup);
                        itemGroup.Item.Carrier = Source;

                        Game.Map.RemoveItem(itemGroup);

                        // TODO: Tell the player only if they can see / notice this
                        Game.MessageHandler.AddMessage($"{Source} picks up a {itemGroup.Item.Name}.");
                    }
                    break;
            }
        }
    }
}
