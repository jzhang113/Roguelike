using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ApplyCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = Utils.Constants.FULL_TURN;
        public IAnimation Animation => _usableItem?.ApplySkill?.Animation;

        private readonly IUsable _usableItem;
        private readonly IEnumerable<Terrain> _target;

        public ApplyCommand(Actor source, IUsable item, IEnumerable<Terrain> targets)
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
