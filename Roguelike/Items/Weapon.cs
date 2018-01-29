using Roguelike.Skills;

namespace Roguelike.Items
{
    abstract class Weapon : Item
    {
        public override char Symbol { get; set; } = '(';

        public override void Equip()
        {
            System.Diagnostics.Debug.Assert(Carrier != null);

            // TODO 1: Attacks should scale with stats.
            Carrier.BasicAttack = new DamageSkill(AttackSpeed, Damage);
            Game.MessageHandler.AddMessage("You wield the weapon.");
        }
        
        public override void Unequip()
        {
            // HACK: Reset the old basic attack properly.
            Carrier.BasicAttack = new DamageSkill(100, 100);
            Game.MessageHandler.AddMessage("You unwield the weapon.");
        }
    }
}
