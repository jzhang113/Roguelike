using Roguelike.Actions;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    internal class ActionCommand : ICommand
    {
        public ISchedulable Source { get; }
        public int EnergyCost { get; }
        public IAnimation Animation => _action.Animation;

        private readonly IAction _action;
        private readonly IEnumerable<Tile> _targets;

        public ActionCommand(ISchedulable source, IAction action, IEnumerable<Tile> targets)
        {
            System.Diagnostics.Debug.Assert(source != null);
            System.Diagnostics.Debug.Assert(action != null);
            System.Diagnostics.Debug.Assert(targets != null);

            Source = source;
            EnergyCost = action.Speed;

            _action = action;
            _targets = targets;
        }

        public ActionCommand(ISchedulable source, IAction action, Tile target) :
            this(source, action, new[] { target }) { }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            foreach (Tile tile in _targets)
            {
                if (Game.Map.TryGetActor(tile.X, tile.Y, out Actors.Actor actor) && actor == Game.Player)
                {
                    // TODO: implement a general reaction system
                    Game.StateHandler.PushState(new State.QteState());
                }

                _action.Activate(Source, tile);
            }
        }
    }
}