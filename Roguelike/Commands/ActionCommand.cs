using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Systems;
using System.Collections.Generic;
using Roguelike.Actions;
using Roguelike.State;

namespace Roguelike.Commands
{
    class ActionCommand : ITargetCommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }
        public IEnumerable<Terrain> Target { get; set; }

        private readonly IAction _action;

        public ActionCommand(Actor source, IAction action, IEnumerable<Terrain> targets = null)
        {
            Source = source;
            EnergyCost = action.Speed;
            Target = targets;

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
                    if (_action.Area.Target != null)
                    {
                        Target = _action.Area.GetTilesInRange(Source);
                    }
                    else
                    {
                        Game.StateHandler.PushState(new TargettingState(this, Source, _action));
                        return new RedirectMessage(false);
                    }
                }
            }
            else
            {
                if (Target == null)
                    Target = _action.Area.GetTilesInRange(Source);
            }

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