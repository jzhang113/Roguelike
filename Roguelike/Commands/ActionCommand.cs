using Roguelike.Actions;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ActionCommand : ICommand
    {
        public ISchedulable Source { get; }
        public int EnergyCost { get; }
        public IAnimation Animation => _action?.Animation;

        private readonly IAction _action;
        private readonly bool _singleTarget;
        private readonly Terrain _target;
        private readonly IEnumerable<Terrain> _targets;

        public ActionCommand(ISchedulable source, IAction action, IEnumerable<Terrain> targets)
        {
            System.Diagnostics.Debug.Assert(action != null);

            Source = source;
            EnergyCost = action.Speed;

            _action = action;
            _singleTarget = false;
            _targets = targets;
        }

        public ActionCommand(ISchedulable source, IAction action, Terrain target)
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