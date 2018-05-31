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
            Carrier.Equipment.Equip(this);
            Game.MessageHandler.AddMessage($"You wield a {Name}.");
        }
        
        public void Unequip()
        {
            Carrier.Equipment.Unequip(this);
            Game.MessageHandler.AddMessage($"You unwield a {Name}.");
        }
    }
}
