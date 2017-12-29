using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class DamageSkill : ISkill
    {
        public int Speed => 10;
        public int Power => 100;

        public void Activate()
        {
            // No additional effects
        }
    }
}
