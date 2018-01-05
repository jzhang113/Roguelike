using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class AttackAction : IAction
    {
        public IActor Source { get; }
        public int EnergyCost { get; set; }

        private int _power;
        private IActor _target;
        private ISkill _skill;

        public AttackAction(IActor source, IActor target, ISkill attack)
        {
            _skill = attack;
            _power = attack.Power + source.STR;
            _target = target;

            Source = source;
            EnergyCost = attack.Speed;
        }

        public void Execute()
        {
            _skill.Activate();

            if (_target != null && _target != Source)
            {
                Game.Map.Highlight[_target.X, _target.Y] = RLNET.RLColor.Blue;
                Game.Map.Highlight[Source.X, Source.Y] = RLNET.RLColor.Red;
                System.Console.WriteLine(_target.X + " " + _target.Y + "\t" + Source.X + " " + Source.Y);

                int damage = _target.TakeDamage(_power);
                Game.MessageHandler.AddMessage(string.Format("{0} attacked {1} for {2} damage", Source.Name, _target.Name, damage));

                if (_target.IsDead())
                {
                    Game.MessageHandler.AddMessage(_target.Name + " is dead");
                    _target.TriggerDeath();
                }
            }
        }
    }
}
