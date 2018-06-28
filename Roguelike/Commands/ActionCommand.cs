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
        private readonly IEnumerable<Terrain> _target;

        public ActionCommand(Actor source, IAction action, IEnumerable<Terrain> targets)
        {
            Source = source;
            EnergyCost = action.Speed;

            _action = action;
            _target = targets;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            foreach (Terrain tile in _target)
            {
                _action.Activate(Source, tile);
            }
        }
    }
}