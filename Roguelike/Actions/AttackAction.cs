using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Skills;
using Roguelike.Systems;

namespace Roguelike.Core
{
    class AttackAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        private int _power;
        private Actor _target;
        private Skill _skill;

        public AttackAction(Actor source, Actor target, Skill attack)
        {
            _skill = attack;
            _power = attack.Power + source.STR;
            _target = target;

            Source = source;
            EnergyCost = attack.Speed;
        }

        public RedirectMessage Validate()
        {
            if (_target == null)
            {
                // TODO: Message handler should probably distinguish when you do stuff vs when enemies do stuff
                Game.MessageHandler.AddMessage(string.Format("{0} attacks thin air.", Source), OptionHandler.MessageLevel.Normal);
                return new RedirectMessage(true);
            }

            if (_target == Source)
            {
                // Q: Should this even be allowed?
                Game.MessageHandler.AddMessage(string.Format("{0} tried to attack itself!", Source), OptionHandler.MessageLevel.Normal);
                return new RedirectMessage(false);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            // TODO: still gotta work out the relationship between attacks and skill
            _skill.Activate(_target);
        }
    }
}
