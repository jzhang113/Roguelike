using Optional;
using Roguelike.Actions;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using Roguelike.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace Roguelike.Commands
{
    internal class DelayActionCommand : ICommand
    {
        public int EnergyCost => _action.EnergyCost;
        public Option<IAnimation> Animation => _action.Animation;

        private readonly ISchedulable _source;
        private readonly IAction _action;
        private readonly IEnumerable<Tile> _targets;

        public DelayActionCommand(ISchedulable source, IAction action, IEnumerable<Tile> targets)
        {
            _source = source;
            _action = action;
            _targets = targets;
        }

        public DelayActionCommand(ISchedulable source, IAction action, Tile target) :
            this(source, action, new[] { target }) { }

        public RedirectMessage Validate() => new RedirectMessage(true);

        public void Execute() => Game.EventScheduler.AddActor(new DelayAttack(_source, _action, _targets));
    }
}