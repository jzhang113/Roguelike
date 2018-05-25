using Roguelike.Interfaces;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Items
{
    abstract class Weapon : Item, IEquipable
    {
        protected Weapon()
        {
            Symbol = '(';
            Abilities = new List<IAction>();
        }

        public void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier.Equipment.IsDefaultWeapon());
            Carrier.Equipment.PrimaryWeapon = this;

            Game.MessageHandler.AddMessage(string.Format("You wield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
        
        public void Unequip()
        {
            Carrier.Equipment.PrimaryWeapon = Carrier.Equipment.DefaultWeapon;

            Game.MessageHandler.AddMessage(string.Format("You unwield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
    }
}
