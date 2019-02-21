using Optional;
using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Items;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    internal class DropCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => 0;
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        public string Input { get; set; }

        private readonly Item _item;
        private int _dropAmount;

        public DropCommand(Actor source, Item item, int dropAmount)
        {
            Source = source;

            _item = item;
            _dropAmount = dropAmount;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_dropAmount > 0);
            if (_dropAmount > _item.Count)
                _dropAmount = _item.Count;

            Item dropped = Source.Inventory.Split(_item, _dropAmount);
            dropped.Loc = Source.Loc;
            Game.Map.AddItem(dropped);
            Game.MessageHandler.AddMessage($"You drop {dropped}.");
        }
    }
}
