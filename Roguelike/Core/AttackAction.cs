using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class AttackAction : IAction
    {
        private int _power;
        private IActor _target;
        private ISkill _skill;

        public int Time { get; set; }

        public AttackAction(IActor source, IActor target, ISkill attack)
        {
            _skill = attack;
            _power = attack.Power + source.STR;
            Time = attack.Speed - source.Speed;
            _target = target;
        }

        public void Execute()
        {
            _skill.Activate();

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
