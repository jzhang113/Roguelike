using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Actions;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class AttackCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        private int _power;
        private IEnumerable<Terrain> _target;
        private Skill _skill;

        public AttackCommand(Actor source, Skill attack, IEnumerable<Terrain> targets = null)
        {
            _skill = attack;
            _power = attack.Power + source.STR;
            _target = targets;

            Source = source;
            EnergyCost = attack.Speed;
        }

        public AttackCommand(Actor source, Skill attack, Terrain target)
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
            System.Diagnostics.Debug.Assert(_skill != null);
            if (_skill == null)
                return new RedirectMessage(false);

            if (_skill.Area.Aimed)
            {
                if (_target == null)
                {
                    InputHandler.BeginTargetting(this, _skill);
                    return new RedirectMessage(false);
                }
            }
            else
            {
                if (_target == null)
                    _target = _skill.Area.GetTilesInRange(Source);
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            _skill.Activate(_target);
        }
    }
}
