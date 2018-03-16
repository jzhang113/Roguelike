using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ActionCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => 0;
        public IEnumerable<Terrain> Target { get; internal set; }

        private IAction _action;

        public ActionCommand(Actor source, IAction action)
        {
            Source = source;
            _action = action;
        }

        public RedirectMessage Validate()
        {
            System.Diagnostics.Debug.Assert(_action != null);
            if (_action == null)
                return new RedirectMessage(false);

            if (_action.Area.Aimed)
            {
                if (Target == null)
                {
                    InputHandler.BeginTargetting(this, _action);
                    return new RedirectMessage(false);
                }
            }
            else
            {
                if (Target == null)
                    Target = _action.Area.GetTilesInRange(Source);
            }

            Source.ActionSequence.Advance();
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            foreach (Terrain tile in Target)
            {
                _action.Activate(Source, tile);
            }
        }
    }
}