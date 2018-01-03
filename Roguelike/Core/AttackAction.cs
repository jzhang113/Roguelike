using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class AttackAction : IAction
    {
        public IActor Source { get; }
        public int Time { get; set; }

        private int _power;
        private IActor _target;
        private ISkill _skill;

        public AttackAction(IActor source, IActor target, ISkill attack)
        {
            _skill = attack;
            _power = attack.Power + source.STR;
            _target = target;

            Source = source;
            Time = attack.Speed - source.Speed + source.QueuedTime;
        }

        public void Execute()
        {
            _skill.Activate();

            if (_target != null)
            {
                int damage = _target.TakeDamage(_power);
                Game.MessageHandler.AddMessage(Source.Name + " attacked " + _target.Name + " for " + damage + " damage");

                if (_target.IsDead())
                {
                    Game.MessageHandler.AddMessage(_target.Name + " is dead");
                    _target.TriggerDeath();
                }
            }
        }
    }
}
