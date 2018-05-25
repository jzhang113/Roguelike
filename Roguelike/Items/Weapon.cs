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

            Game.MessageHandler.AddMessage($"You wield a {Name}.");
        }
        
        public void Unequip()
        {
            Carrier.Equipment.PrimaryWeapon = Carrier.Equipment.DefaultWeapon;

            Game.MessageHandler.AddMessage($"You unwield a {Name}.");
        }
    }
}
