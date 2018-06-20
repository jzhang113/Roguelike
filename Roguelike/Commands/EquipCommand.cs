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
        private Item _item;

        public EquipCommand(Actor source, char key)
        {
            Source = source;
            _key = key;
        }

        public RedirectMessage Validate()
        {
            if (!Source.Inventory.TryGetKey(_key, out _item))
            {
                Game.MessageHandler.AddMessage("No such item to equip.");
                return new RedirectMessage(false);
            }

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
            
            IEquipable equipable = (IEquipable)Source.Inventory.Split(_item, 1);
            equipable.Equip(Source);
        }
    }
}
