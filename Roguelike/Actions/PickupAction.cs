using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Items;
using System.Collections.Generic;
using Roguelike.Systems;

namespace Roguelike.Actions
{
    class PickupAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 50;

        private InventoryHandler _itemStack;

        public PickupAction(Actor source, InventoryHandler itemStack)
        {
            Source = source;
            _itemStack = itemStack;
        }

        public RedirectMessage Validate()
        {
            // Trying to pick up an empty tile.
            if (_itemStack == null || _itemStack.IsEmpty())
            {
                Game.MessageHandler.AddMessage("There's nothing to pick up here.", OptionHandler.MessageLevel.Normal);
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            Item obj;

            switch(_itemStack.Size())
            {
                case 1:
                    obj = _itemStack.GetItem('a');
                    Source.Inventory.Add(obj);

                    Game.Map.RemoveItem(obj);
                    Game.MessageHandler.AddMessage(string.Format("You pick up a {0}.", obj.Name), OptionHandler.MessageLevel.Normal);
                    break;
                default:
                    if (Source is Player)
                    {
                        // HACK: handle pickup menu - this placeholder at least lets you pick up the top item
                        obj = _itemStack.GetItem('a');
                        Source.Inventory.Add(obj);

                        Game.Map.RemoveItem(obj);
                        Game.MessageHandler.AddMessage(string.Format("You pick up a {0}.", obj.Name), OptionHandler.MessageLevel.Normal);
                    }
                    else
                    {
                        // HACK: Monsters will simply grab the top item off of a pile if they try to pick stuff up.
                        obj = _itemStack.GetItem('a');
                        Source.Inventory.Add(obj);

                        Game.Map.RemoveItem(obj);

                        // TODO: Tell the player only if they can see / notice this
                        Game.MessageHandler.AddMessage(string.Format("{0} picks up a {1}.", Source, obj.Name), OptionHandler.MessageLevel.Normal);
                    }
                    break;
            }
        }
    }
}
