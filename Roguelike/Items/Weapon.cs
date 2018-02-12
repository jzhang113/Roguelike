using Roguelike.Actors;
using Roguelike.Skills;
using Roguelike.Systems;

namespace Roguelike.Items
{
    public abstract class Weapon : Item
    {
        public override char Symbol { get; set; } = '(';

        public override void Equip(Actor actor)
        {
            System.Diagnostics.Debug.Assert(actor != null);

            // TODO 1: Attacks should scale with stats.
            actor.BasicAttack = new DamageSkill(AttackSpeed, Damage);

            Game.MessageHandler.AddMessage(string.Format("You wield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
        
        public override void Unequip(Actor actor)
        {
            // HACK: Reset the old basic attack properly.
            actor.BasicAttack = new DamageSkill(100, 100);

            Game.MessageHandler.AddMessage(string.Format("You unwield a {0}.", Name), OptionHandler.MessageLevel.Normal);
        }
    }
}
