using Optional;
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
        public Option<IAnimation> Animation => _action.Animation;

        private readonly IAction _action;
        private readonly IEnumerable<Tile> _targets;

        public ActionCommand(ISchedulable source, IAction action, IEnumerable<Tile> targets)
        {
            System.Diagnostics.Debug.Assert(source != null);
            System.Diagnostics.Debug.Assert(action != null);
            System.Diagnostics.Debug.Assert(targets != null);

            Source = source;
            EnergyCost = action.EnergyCost;

            _action = action;
            _targets = targets;
        }

        public ActionCommand(ISchedulable source, IAction action, Tile target) :
            this(source, action, new[] { target }) { }

        public RedirectMessage Validate() => new RedirectMessage(true);

        public void Execute()
        {
            foreach (Tile tile in _targets)
            {
                bool activate = true;
                Game.Threatened.Unset(tile.X, tile.Y);

                Game.Map.GetActor(tile.X, tile.Y).MatchSome(actor =>
                {
                    Actors.ReactionMessage reaction = actor.GetReaction();
                    if (reaction.Command != null)
                    {
                        // HACK: eww, this reaches into what should be a private method
                        // TODO: should reactions be free / cheaper than a full action
                        // TODO: what about reactions that negate damage?
                        EventScheduler.Execute(actor, reaction.Command);
                    }
                    else if (reaction.Delayed)
                    {
                        Game.EventScheduler.Stop();
                    }

                    if (reaction.Negating)
                        activate = false;
                });

                if (activate)
                    _action.Activate(Source, tile);
            }
        }
    }
}