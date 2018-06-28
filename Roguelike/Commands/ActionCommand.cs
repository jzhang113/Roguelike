using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ActionCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        private readonly IAction _action;
        private readonly bool _singleTarget;
        private readonly Terrain _target;
        private readonly IEnumerable<Terrain> _targets;

        public ActionCommand(Actor source, IAction action, IEnumerable<Terrain> targets)
        {
            System.Diagnostics.Debug.Assert(action != null);

            Source = source;
            EnergyCost = action.Speed;

            _action = action;
            _singleTarget = false;
            _targets = targets;
        }

        public ActionCommand(Actor source, IAction action, Terrain target)
        {
            System.Diagnostics.Debug.Assert(action != null);

            Source = source;
            EnergyCost = action.Speed;

            _action = action;
            _singleTarget = true;
            _target = target;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            if (_singleTarget)
            {
                _action.Activate(Source, _target);
            }
            else
            {
                foreach (Terrain tile in _targets)
                {
                    _action.Activate(Source, tile);
                }
            }
        }
    }
}