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
        private Item _item;

        public DropCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (Source.Inventory.TryGetKey(_key, out _item))
                return new RedirectMessage(true);

            Game.MessageHandler.AddMessage("No such item to drop.");
            return new RedirectMessage(false);
        }

        public void Execute()
        {
            Item item = _item;
            Item dropped = Source.Inventory.Split(_item, _item.Count);

            item.X = Source.X;
            item.Y = Source.Y;
            Game.Map.AddItem(dropped);
            Game.MessageHandler.AddMessage($"You drop a {item.Name}.");
        }
    }
}
