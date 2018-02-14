using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Skills
{
    class DamageSkill : ISkill
    {
        public Actor Source { get; private set; }
        public int Speed { get; private set; }
        public int Power { get; private set; }

        public DamageSkill(Actor source, int speed, int power)
        {
            Source = source;
            Speed = speed;
            Power = power;
        }

        // Deals tamage to the target.
        public void Activate(Actor target)
        {
            int damage = target.TakeDamage(Power);
            Game.MessageHandler.AddMessage(string.Format("{0} attacked {1} for {2} damage", Source.Name, target.Name, damage), OptionHandler.MessageLevel.Normal);
        }
    }
}
