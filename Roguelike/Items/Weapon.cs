using Roguelike.Interfaces;
using System;

namespace Roguelike.Items
{
    [Serializable]
    public class Weapon : Item, IEquippable
    {
        public Weapon(ItemParameter parameters, RLNET.RLColor color) : base(parameters, color, '(')
        {
        }

        public Weapon(Weapon other) : base(other)
        {
        }

        public void Equip(IEquipped equipped)
        {
            System.Diagnostics.Debug.Assert(equipped != null);

            if (!equipped.Equipment.IsDefaultWeapon())
            {
                Game.MessageHandler.AddMessage(
                    $"You are already wielding a {equipped.Equipment.PrimaryWeapon.Name}!");
                return;
            }

            equipped.Equipment.Equip(this);
            Game.MessageHandler.AddMessage($"You wield a {Name}.");
        }
        
        public void Unequip(IEquipped equipped)
        {
            System.Diagnostics.Debug.Assert(equipped != null);

            equipped.Equipment.Unequip();
            Game.MessageHandler.AddMessage($"You unwield a {Name}.");
        }

        public override Item DeepClone()
        {
            return new Weapon(this);
        }
    }
}
