using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class Attack : IAction
    {
        private int power;

        public Attack(int power)
        {
            this.power = power;
        }

        public void Execute(IActor target)
        {
            if (target != null)
            {
                int damage = target.TakeDamage(power);
                Game.MessageHandler.AppendMessage("for " + damage + " damage");

                if (target.IsDead())
                {
                    Game.MessageHandler.AddMessage(target.Name + " is dead");
                    target.TriggerDeath();
                }
            }
        }
    }
}
