using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class DropCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;

        private char _key;

        public DropCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.HasKey(_key))
            {
                Game.MessageHandler.AddMessage("No such item to drop.");
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            ItemInfo itemGroup = Source.Inventory.GetItem(_key);
            Item item = itemGroup.Item;

            Source.Inventory.Remove(itemGroup);
            item.Carrier = null;

            item.X = Source.X;
            item.Y = Source.Y;
            Game.Map.AddItem(itemGroup);
            Game.MessageHandler.AddMessage($"You drop a {item.Name}.");
        }
    }
}
