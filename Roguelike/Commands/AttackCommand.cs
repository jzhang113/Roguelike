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
        public int EnergyCost { get; } = 0;

        private IEnumerable<Terrain> _target;
        private ActionSequence _skill;

        public AttackCommand(Actor source, ActionSequence attack, IEnumerable<Terrain> targets = null)
        {
            _skill = attack;
            _target = targets;

            Source = source;
            // EnergyCost = attack.Speed;
        }

        public AttackCommand(Actor source, ActionSequence attack, Terrain target)
        {
            _skill = attack;
            _target = new List<Terrain>
            {
                target
            };

            Source = source;
            // EnergyCost = attack.Speed;
        }

        public RedirectMessage Validate()
        {
            System.Diagnostics.Debug.Assert(_skill != null);
            if (_skill == null)
                return new RedirectMessage(false);

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            Source.ActiveSequence = true;
            Source.ActionSequence = _skill;
        }
    }
}
