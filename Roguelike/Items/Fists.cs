using Roguelike.Actors;
using System;

namespace Roguelike.Items
{
    [Serializable]
    class Fists : Weapon
    {
        // Default weapon.
        public Fists(Actor actor)
        {
            Name = "fists";
            Carrier = actor;

            // TODO 2: Allow for variable formulaes.
            AttackSpeed = 120;
            Damage = 100;
            MeleeRange = 1.5f;
            ThrowRange = 1;
        }
    }
}
