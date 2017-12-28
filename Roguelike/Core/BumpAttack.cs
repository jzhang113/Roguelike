using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class BumpAttack : IAction
    {
        private int damage;

        public BumpAttack(int damage)
        {
            this.damage = damage;
        }

        public void Execute(IActor target)
        {
            if (target != null)
            {
                target.TakeDamage(damage);

                if (target.IsDead())
                {
                    target.TriggerDeath();
                }
            }
        }
    }
}
