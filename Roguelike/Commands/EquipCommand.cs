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
        private ItemInfo _itemGroup;

        public EquipCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.TryGetKey(_key, out _itemGroup))
            {
                Game.MessageHandler.AddMessage("No such item to equip.");
                return new RedirectMessage(false);
            }

            if (_itemGroup.Item is IEquipable)
            {
                return new RedirectMessage(true);
            }
            else
            {
                Game.MessageHandler.AddMessage($"Cannot equip {_itemGroup.Item.Name}.");
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_itemGroup != null);
            Source.Inventory.Remove(_itemGroup);
            (_itemGroup.Item as IEquipable)?.Equip();
        }
    }
}
