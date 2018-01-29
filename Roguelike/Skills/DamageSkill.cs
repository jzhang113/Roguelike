using Roguelike.Interfaces;

namespace Roguelike.Skills
{
    class DamageSkill : ISkill
    {
        public int Speed { get; private set; }
        public int Power { get; private set; }

        public DamageSkill(int speed, int power)
        {
            Speed = speed;
            Power = power;
        }

        public void Activate()
        {
            // No additional effects
        }
    }
}
