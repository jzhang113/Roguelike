using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike.Items
{
    class Fists : Weapon
    {
        public Fists()
        {
            Name = "fists";

            // TODO 2: Allow for variable formulaes.
            AttackSpeed = 100;
            Damage = 100;
            MeleeRange = 1;
            ThrowRange = 1;
        }
    }
}
