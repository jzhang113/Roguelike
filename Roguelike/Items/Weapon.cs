using Roguelike.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Items
{
    [Serializable]
    public class Weapon : Item, IEquipable
    {
        public Weapon(string name, IMaterial material) : base(name, material)
        {
            DrawingComponent.Symbol = '(';
        }

        protected Weapon(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier.Equipment.IsDefaultWeapon());
            Carrier.Equipment.Equip(this);
            Game.MessageHandler.AddMessage($"You wield a {Name}.");
        }
        
        public void Unequip()
        {
            Carrier.Equipment.Unequip();
            Game.MessageHandler.AddMessage($"You unwield a {Name}.");
        }
    }
}
