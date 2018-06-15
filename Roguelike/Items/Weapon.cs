﻿using Roguelike.Interfaces;
using System;

namespace Roguelike.Items
{
    [Serializable]
    public class Weapon : Item, IEquipable
    {
        public Weapon(string name, IMaterial material) : base(name, material)
        {
            DrawingComponent.Symbol = '(';
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
