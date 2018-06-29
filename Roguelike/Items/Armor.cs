using Roguelike.Actors;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Items
{
    [Serializable]
    public class Armor : Item, IEquipable
    {
        public Enums.ArmorType Type { get; }

        public Armor(ItemParameter parameters, RLNET.RLColor color, Enums.ArmorType type) : base(parameters, color, '[')
        {
            Type = type;
        }

        public Armor(Armor other) : base(other)
        {
            Type = other.Type;
        }

        public void Equip(Actor actor)
        {
            System.Diagnostics.Debug.Assert(actor != null);

            if (actor.Equipment.Armor[Type] != null)
            {
                Game.MessageHandler.AddMessage($"You are already wearing a {actor.Equipment.Armor[Type]}!");
                return;
            }

            actor.Equipment.Equip(this);
            Game.MessageHandler.AddMessage($"You put on the {Name}.");
        }

        public void Unequip(Actor actor)
        {
            System.Diagnostics.Debug.Assert(actor != null);

            actor.Equipment.Unequip(this);
            Game.MessageHandler.AddMessage($"You take off the {Name}.");
        }

        public override Item DeepClone()
        {
            return new Armor(this);
        }
    }
}
