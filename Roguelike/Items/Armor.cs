using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Items
{
    abstract class Armor : Item, IEquipable
    {
        private readonly Enums.ArmorType _type;

        protected Armor(Enums.ArmorType type)
        {
            Symbol = '[';
            _type = type;
        }

        public void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier.Equipment.Armor[_type] == null);
            Carrier.Equipment.Armor[_type] = this;

            Game.MessageHandler.AddMessage($"You put on the {Name}.");
        }

        public void Unequip()
        {
            Carrier.Equipment.Armor[_type] = null;

            Game.MessageHandler.AddMessage($"You take off the {Name}.");
        }
    }
}
