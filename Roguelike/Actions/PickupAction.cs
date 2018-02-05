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

        private IList<Item> _itemStack;

        public PickupAction(Actor source, IList<Item> itemStack)
        {
            Source = source;
            _itemStack = itemStack;
        }

        public RedirectMessage Validate()
        {
            // Trying to pick up an empty tile.
            if (_itemStack == null || _itemStack.Count == 0)
            {
                Game.MessageHandler.AddMessage("There's nothing to pick up here");
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            switch(_itemStack.Count)
            {
                case 1:
                    Item obj = _itemStack[0];
                    Source.Inventory.Add(obj);
                    Game.Map.RemoveItem(obj);
                    Game.MessageHandler.AddMessage(string.Format("You pick up a {0}", obj.Name));
                    break;
                default:
                    // TODO: handle pickup menu
                    break;
            }
        }
    }
}
