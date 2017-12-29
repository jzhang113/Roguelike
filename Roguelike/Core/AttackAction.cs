using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class AttackAction : IAction
    {
        private int _power;
        private IActor _target;
        public int Time { get; set; }

        public AttackAction(IActor source, IActor target, int power, int time)
        {
            _power = power;
            _target = target;
            Time = time;
        }

        public void Execute()
        {
            if (_target != null)
            {
                int damage = _target.TakeDamage(_power);
                Game.MessageHandler.AppendMessage("for " + damage + " damage");

                if (_target.IsDead())
                {
                    Game.MessageHandler.AddMessage(_target.Name + " is dead");
                    _target.TriggerDeath();
                }
            }
        }
    }
}
