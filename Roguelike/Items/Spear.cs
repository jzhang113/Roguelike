using Roguelike.Interfaces;
using System;

namespace Roguelike.Items
{
    [Serializable]
    class Spear : Weapon
    {
        public Spear(IMaterial type)
        {
            Name = "spear";
            Material = type;

            // TODO 2: Allow for variable formulaes.
            AttackSpeed = 240;
            Damage = 200;
            MeleeRange = 2;
            ThrowRange = 7;
        }
    }
}
