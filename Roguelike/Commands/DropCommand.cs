using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.State;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class DropCommand : IInputCommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;
        public string Input { get; set; }

        private readonly ItemCount _itemCount;
        private int _dropAmount;

        public DropCommand(Actor source, ItemCount itemCount, string amount = null)
        {
            Source = source;
            _itemCount = itemCount;

            if (int.TryParse(amount, out int dropAmount))
            {
                Input = amount;
                _dropAmount = dropAmount;
            }
            else
            {
                Input = null;
            }
        }

        public RedirectMessage Validate()
        {
            if (_itemCount.Count == 1)
            {
                _dropAmount = 1;
                return new RedirectMessage(true);
            }

            if (_dropAmount > 0)
                return new RedirectMessage(true);

            if (Input == null)
            {
                Game.StateHandler.PushState(new TextInputState(this));
                return new RedirectMessage(false);
            }

            if (int.TryParse(Input, out _dropAmount) && _dropAmount > 0)
                return new RedirectMessage(true);

            Game.MessageHandler.AddMessage($"Unknown amount: {Input}");
            return new RedirectMessage(false);
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
