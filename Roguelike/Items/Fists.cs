using Roguelike.Actors;

namespace Roguelike.Items
{
    class Fists : Weapon
    {
        // Default weapon.
        public Fists(Actor actor)
        {
            Name = "fists";
            Carrier = actor;

            // TODO 2: Allow for variable formulaes.
            AttackSpeed = 100;
            Damage = 100;
            MeleeRange = 1;
            ThrowRange = 1;
        }
    }
}
