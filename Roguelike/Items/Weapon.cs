using Roguelike.Skills;

namespace Roguelike.Items
{
    public abstract class Weapon : Item
    {
        public override char Symbol { get; set; } = '(';

        public override void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier != null);

            // TODO 1: Attacks should scale with stats.
            Carrier.BasicAttack = new DamageSkill(AttackSpeed, Damage);
            Game.MessageHandler.AddMessage("You wield the weapon.", Systems.OptionHandler.MessageLevel.Normal);
        }
        
        public override void Unequip()
        {
            // HACK: Reset the old basic attack properly.
            Carrier.BasicAttack = new DamageSkill(100, 100);
            Game.MessageHandler.AddMessage("You unwield the weapon.", Systems.OptionHandler.MessageLevel.Normal);
        }
    }
}
