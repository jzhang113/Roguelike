using Roguelike.Interfaces;
using System;

namespace Roguelike.Items
{
    [Serializable]
    class HeavyArmor : Armor
    {
        public HeavyArmor(IMaterial type) : base(Enums.ArmorType.Armor)
        {
            Name = "heavy armor";
            Material = type;

            // TODO 2: Allow for variable formulaes.
            AttackSpeed = 1000;
            Damage = 100;
            MeleeRange = 1;
            ThrowRange = 3;
        }
    }
}
