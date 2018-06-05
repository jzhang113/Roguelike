using Roguelike.Interfaces;
using Roguelike.Systems;
using System;
using System.Collections.Generic;

namespace Roguelike.Items
{
    [Serializable]
    public class Weapon : Item, IEquipable
    {
        public Weapon(string name, IMaterial material) : base(name, material)
        {
            Symbol = '(';
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
