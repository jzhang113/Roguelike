using Roguelike.Actors;

namespace Roguelike.Skills
{
    class DamageSkill : Skill
    {
        public DamageSkill(Actor source, int speed, int power) : base(source, speed, power)
        {
        }

        // Deals tamage to the target.
        public override void Activate(Actor target)
        {
            int damage = target.TakeDamage(Power);

            if (target.IsDead)
                target.State = Core.ActorState.Dead;

            Game.MessageHandler.AddMessage(string.Format("{0} attacked {1} for {2} damage", Source.Name, target.Name, damage), Systems.OptionHandler.MessageLevel.Normal);
        }
    }
}
