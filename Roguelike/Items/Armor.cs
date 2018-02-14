using Roguelike.Interfaces;

namespace Roguelike.Items
{
    public abstract class Armor : Item, IEquipable
    {
        protected Armor()
        {
            Symbol = '[';
        }

        public void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier.Armor == null);
            Carrier.Armor = this;

            Game.MessageHandler.AddMessage("You put on the armor.", Systems.OptionHandler.MessageLevel.Normal);
        }

        public void Unequip()
        {
            Carrier.Armor = null;

            Game.MessageHandler.AddMessage("You take off the armor.", Systems.OptionHandler.MessageLevel.Normal);
        }
    }
}
