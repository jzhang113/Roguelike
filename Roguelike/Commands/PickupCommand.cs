using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Items;
using Roguelike.Systems;
using System.Linq;

namespace Roguelike.Commands
{
    internal class PickupCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => Data.Constants.HALF_TURN;
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
            Item item, split;

            switch(_itemStack.Count)
            {
                case 1:
                    item = _itemStack.First();
                    split = Game.Map.SplitItem(item);
                    Source.Inventory.Add(split);
                    Game.MessageHandler.AddMessage($"You pick up {split}.");
                    break;
                default:
                    if (Source is Player)
                    {
                        // HACK: handle pickup menu - this placeholder at least lets you pick up the top item
                        item = _itemStack.First();
                        split = Game.Map.SplitItem(item);
                        Source.Inventory.Add(split);
                        Game.MessageHandler.AddMessage($"You pick up {split}.");
                    }
                    else
                    {
                        // HACK: Monsters will simply grab the top item off of a pile if they try to pick stuff up.
                        item = _itemStack.First();
                        split = Game.Map.SplitItem(item);
                        Source.Inventory.Add(split);
                        // TODO: Tell the player only if they can see / notice this
                        Game.MessageHandler.AddMessage($"{Source} picks up {split}.");
                    }
                    break;
            }
        }
    }
}
