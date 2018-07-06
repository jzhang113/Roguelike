using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Items;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class DropCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;
        public IAnimation Animation { get; } = null;

        public string Input { get; set; }

        private readonly ItemCount _itemCount;
        private int _dropAmount;

        public DropCommand(Actor source, ItemCount itemCount, int dropAmount)
        {
            Source = source;

            _itemCount = itemCount;
            _dropAmount = dropAmount;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_dropAmount > 0);
            if (_dropAmount > _itemCount.Count)
                _dropAmount = _itemCount.Count;
            ItemCount dropped = Source.Inventory.Split(new ItemCount { Item = _itemCount.Item, Count = _dropAmount });

            dropped.Item.X = Source.X;
            dropped.Item.Y = Source.Y;
            Game.Map.AddItem(dropped);
            Game.MessageHandler.AddMessage($"You drop {dropped}.");
        }
    }
}
