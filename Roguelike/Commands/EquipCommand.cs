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

            // TODO: probably should check for equippability in a better manner
            Item item = Source.Inventory.GetItem(_key);
            if (item is IEquipable)
            {
                return new RedirectMessage(true);
            }
            else
            {
                Game.MessageHandler.AddMessage($"Cannot equip {item.Name}.");
                return new RedirectMessage(false);
            }
        }

        public void Execute()
        {
            Item item = Source.Inventory.GetItem(_key);
            Source.Inventory.Remove(item);
            (item as IEquipable).Equip();
        }
    }
}
