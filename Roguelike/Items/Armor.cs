using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Items
{
    abstract class Armor : Item, IEquipable
    {
        private ArmorType _type;

        protected Armor(ArmorType type)
        {
            Symbol = '[';
            _type = type;
        }

        public void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier.Equipment.Armor[_type] == null);
            Carrier.Equipment.Armor[_type] = this;

            Game.MessageHandler.AddMessage(string.Format("You put on the {0}.", Name), OptionHandler.MessageLevel.Normal);
        }

        public void Unequip()
        {
            Carrier.Equipment.Armor[_type] = null;

            Game.MessageHandler.AddMessage(string.Format("You take off the {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
    }
}
