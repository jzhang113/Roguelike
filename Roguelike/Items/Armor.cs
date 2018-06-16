using Roguelike.Interfaces;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Items
{
    [Serializable]
    public class Armor : Item, IEquipable
    {
        public Enums.ArmorType Type { get; }

        public Armor(string name, IMaterial material, Enums.ArmorType type) : base(name, material)
        {
            DrawingComponent.Symbol = '[';
            Type = type;
        }

        protected Armor(SerializationInfo info, StreamingContext context) : base(info, context)
        {
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
