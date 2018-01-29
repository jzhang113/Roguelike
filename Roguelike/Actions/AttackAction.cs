using Roguelike.Actors;
using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class AttackAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        private int _power;
        private Actor _target;
        private ISkill _skill;

        public AttackAction(Actor source, Actor target, ISkill attack)
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

            System.Diagnostics.Debug.Assert(_target != null);
            if (_target != Source)
            {
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
