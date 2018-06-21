using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class DropCommand : IInputCommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;
        public string Input { get; set; }

        private readonly char _key;
        private Item _item;
        private int _dropAmount;

        public DropCommand(Actor source, char key, string amount = null)
        {
            Source = source;
            _key = key;
            Input = null;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.TryGetKey(_key, out _item))
            {
                Game.MessageHandler.AddMessage("No such item to drop.");
                return new RedirectMessage(false);
            }

            System.Diagnostics.Debug.Assert(_item.Count > 0);
            if (_item.Count == 1)
            {
                _dropAmount = 1;
                return new RedirectMessage(true);
            }

            if (Input == null)
            {
                InputHandler.BeginTextInput(this);
                return new RedirectMessage(false);
            }

            if (!int.TryParse(Input, out _dropAmount) || _dropAmount <= 0)
            {
                Game.MessageHandler.AddMessage($"Unknown amount: {Input}");
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_dropAmount > 0);
            if (_dropAmount > _item.Count)
                _dropAmount = _item.Count;
            Item dropped = Source.Inventory.Split(_item, _dropAmount);

            dropped.X = Source.X;
            dropped.Y = Source.Y;
            Game.Map.AddItem(dropped);
            Game.MessageHandler.AddMessage($"You drop {_dropAmount} {_item.Name}.");
        }
    }
}
