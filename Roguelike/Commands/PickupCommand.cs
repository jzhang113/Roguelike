using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Items;
using Roguelike.Systems;
using System.Linq;

namespace Roguelike.Commands
{
    class PickupCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => Utils.Constants.HALF_TURN;
        public IAnimation Animation { get; } = null;

        private readonly InventoryHandler _itemStack;

        public PickupCommand(Actor source, InventoryHandler itemStack)
        {
            System.Diagnostics.Debug.Assert(source != null);
            System.Diagnostics.Debug.Assert(itemStack != null);

            Source = source;
            _itemStack = itemStack;
        }

        public RedirectMessage Validate()
        {
            // Trying to pick up an empty tile.
            if (_itemStack.IsEmpty())
            {
                Game.MessageHandler.AddMessage("There's nothing to pick up here.");
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            ItemCount itemCount;

            switch(_itemStack.Count)
            {
                case 1:
                    itemCount = _itemStack.First();
                    Source.Inventory.Add(itemCount);

                    Game.Map.RemoveItem(itemCount);
                    Game.MessageHandler.AddMessage($"You pick up {itemCount}.");
                    break;
                default:
                    if (Source is Player)
                    {
                        // HACK: handle pickup menu - this placeholder at least lets you pick up the top item
                        itemCount = _itemStack.First();
                        Source.Inventory.Add(itemCount);

                        Game.Map.RemoveItem(itemCount);
                        Game.MessageHandler.AddMessage($"You pick up {itemCount}.");
                    }
                    else
                    {
                        // HACK: Monsters will simply grab the top item off of a pile if they try to pick stuff up.
                        itemCount = _itemStack.First();
                        Source.Inventory.Add(itemCount);

                        Game.Map.RemoveItem(itemCount);

                        // TODO: Tell the player only if they can see / notice this
                        Game.MessageHandler.AddMessage($"{Source} picks up {itemCount}.");
                    }
                    break;
            }
        }
    }
}
