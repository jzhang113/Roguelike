using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Items;
using System.Collections.Generic;

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

        public void Execute()
        {
            if (_itemStack == null)
            {
                Game.MessageHandler.AddMessage("There's nothing to pick up here");
                return;
            }

            switch(_itemStack.Count)
            {
                case 0:
                    Game.MessageHandler.AddMessage("There's nothing to pick up here");
                    break;
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
