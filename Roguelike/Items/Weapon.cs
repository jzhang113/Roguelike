using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Items
{
    public abstract class Weapon : Item, IEquipable
    {
        protected Weapon()
        {
            Symbol = '(';
        }

        public void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier.Weapon == Carrier.DefaultWeapon);
            Carrier.Weapon = this;

            Game.MessageHandler.AddMessage(string.Format("You wield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
        
        public void Unequip()
        {
            Carrier.Weapon = Carrier.DefaultWeapon;

            Game.MessageHandler.AddMessage(string.Format("You unwield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
    }
}
