using Roguelike.Actors;
using Roguelike.Interfaces;
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

        public void Equip(Actor actor)
        {
            System.Diagnostics.Debug.Assert(actor != null);

            if (!actor.Equipment.IsDefaultWeapon())
            {
                Game.MessageHandler.AddMessage($"You are already wielding a {actor.Equipment.PrimaryWeapon.Name}!");
                return;
            }

            actor.Equipment.Equip(this);
            Game.MessageHandler.AddMessage($"You wield a {Name}.");
        }
        
        public void Unequip(Actor actor)
        {
            System.Diagnostics.Debug.Assert(actor != null);

            actor.Equipment.Unequip();
            Game.MessageHandler.AddMessage($"You unwield a {Name}.");
        }
    }
}
