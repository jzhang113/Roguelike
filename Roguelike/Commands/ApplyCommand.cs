using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    internal class ApplyCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => Data.Constants.FULL_TURN;
        public IAnimation Animation => _usableItem?.ApplySkill?.Animation;

        private readonly IUsable _usableItem;
        private readonly IEnumerable<Tile> _target;

        public ApplyCommand(Actor source, IUsable item, IEnumerable<Tile> targets)
        {
            Source = source;

            _usableItem = item;
            _target = targets;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            _usableItem.Apply(Source, _target);
        }
    }
}
