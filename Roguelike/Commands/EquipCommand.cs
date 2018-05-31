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

        private char _key;
        private Item _item;

        public EquipCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.HasKey(_key))
            {
                Game.MessageHandler.AddMessage("No such item to equip.");
                return new RedirectMessage(false);
            }

            if (_item == null)
                _item = Source.Inventory.GetItem(_key);

            if (_item is IEquipable)
            {
                return new RedirectMessage(true);
            }
            else
            {
                Game.MessageHandler.AddMessage($"Cannot equip {_item.Name}.");
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_item != null);
            Source.Inventory.Remove(_item);
            (_item as IEquipable).Equip();
        }
    }
}
