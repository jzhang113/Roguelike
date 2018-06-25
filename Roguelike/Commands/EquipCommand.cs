using Roguelike.Interfaces;
using Roguelike.Actors;
using Roguelike.Systems;
using Roguelike.Items;

namespace Roguelike.Commands
{
    class EquipCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;

        private readonly char _key;
        private ItemCount _itemCount;

        public EquipCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.TryGetKey(_key, out _itemCount))
            {
                Game.MessageHandler.AddMessage("No such item to equip.");
                return new RedirectMessage(false);
            }

            if (_itemCount.Item is IEquipable)
            {
                return new RedirectMessage(true);
            }
            else
            {
                Game.MessageHandler.AddMessage($"Cannot equip {_itemCount.Item.Name}.");
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_itemCount?.Item != null);

            ItemCount itemCount = Source.Inventory.Split(new ItemCount {Item = _itemCount.Item, Count = 1});
            IEquipable equipable = (IEquipable)itemCount.Item;
            equipable.Equip(Source);
        }
    }
}
