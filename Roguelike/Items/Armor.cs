using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Items
{
    abstract class Armor : Item, IEquipable
    {
        public Enums.ArmorType Type { get; }

        protected Armor(Enums.ArmorType type)
        {
            Symbol = '[';
            Type = type;
        }

        public void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier.Equipment.Armor[Type] == null);
            Carrier.Equipment.Equip(this);
            Game.MessageHandler.AddMessage($"You put on the {Name}.");
        }

        public void Unequip()
        {
            Carrier.Equipment.Unequip(this);
            Game.MessageHandler.AddMessage($"You take off the {Name}.");
        }
    }
}
