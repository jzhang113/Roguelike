using Roguelike.Actors;

namespace Roguelike.Skills
{
    internal class HealingSkill : Skill
    {
        public HealingSkill(Actor source, int speed, int power) : base(source, speed, power)
        {
        }

        // Heals the target by amount up to its maximum health.
        public override void Activate(Actor target)
        {
            int healing = target.TakeHealing(Power);

            Game.MessageHandler.AddMessage(string.Format("{0} healed {1} damage", Source.Name, healing), Systems.OptionHandler.MessageLevel.Normal);
        }
    }
}