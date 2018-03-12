using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Skills;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Actions
{
    class AttackCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        private int _power;
        private IEnumerable<Terrain> _target;
        private Skill _skill;

        public AttackCommand(Actor source, IEnumerable<Terrain> target, Skill attack)
        {
            _skill = attack;
            _power = attack.Power + source.STR;
            _target = target;

            Source = source;
            EnergyCost = attack.Speed;
        }

        public AttackCommand(Actor source, Terrain target, Skill attack)
        {
            _skill = attack;
            _power = attack.Power + source.STR;
            _target = new List<Terrain>
            {
                target
            };

            Source = source;
            EnergyCost = attack.Speed;
        }

        public RedirectMessage Validate()
        {
            // TODO: redo validation
            /*
            if (_target.Unit == null)
            {
                Game.MessageHandler.AddMessage(string.Format("{0} attacks thin air.", Source), OptionHandler.MessageLevel.Normal);
                return new RedirectMessage(true);
            }

            if (_target.Unit == Source)
            {
                // Q: Should this even be allowed?
                Game.MessageHandler.AddMessage(string.Format("{0} tried to attack itself!", Source), OptionHandler.MessageLevel.Verbose);
                return new RedirectMessage(false, new WaitAction(Source));
            }
            */

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            _skill.Activate(_target);
        }
    }
}
