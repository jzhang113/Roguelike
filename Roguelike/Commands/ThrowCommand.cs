using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ThrowCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        private readonly Item _thrownItem;
        private readonly ICollection<Actor> _targetList;
        private readonly IEnumerable<Terrain> _target;

        public ThrowCommand(Actor source, Item item, IEnumerable<Terrain> targets)
        {
            Source = source;
            EnergyCost = item.Parameters.AttackSpeed;
            _target = targets;

            _thrownItem = item;
            _targetList = new List<Actor>();
        }

        public RedirectMessage Validate()
        {
            foreach (Terrain tile in _target)
            {
                if (Game.Map.TryGetActor(tile.X, tile.Y, out Actor target))
                    _targetList.Add(target);
            }

            return new RedirectMessage(_targetList.Count != 0);
        }

        public void Execute()
        {
            foreach (Actor target in _targetList)
            {
                _thrownItem.Throw(target);
            }
        }
    }
}
