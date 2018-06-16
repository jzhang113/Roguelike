using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class DropCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;

        private readonly char _key;
        private ItemInfo _itemGroup;

        public DropCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (Source.Inventory.TryGetKey(_key, out _itemGroup))
                return new RedirectMessage(true);

            Game.MessageHandler.AddMessage("No such item to drop.");
            return new RedirectMessage(false);
        }

        public void Execute()
        {
            Item item = _itemGroup.Item;

            Source.Inventory.Remove(_itemGroup);
            item.Carrier = null;

            item.X = Source.X;
            item.Y = Source.Y;
            Game.Map.AddItem(_itemGroup);
            Game.MessageHandler.AddMessage($"You drop a {item.Name}.");
        }
    }
}
