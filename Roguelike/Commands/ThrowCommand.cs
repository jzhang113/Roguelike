using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    class ThrowCommand : ITargetCommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }
        public IEnumerable<Terrain> Target { get; set; }

        private readonly Item _thrownItem;
        private readonly ICollection<Actor> _targetList;

        public ThrowCommand(Actor source, Item item, IEnumerable<Terrain> target = null)
        {
            System.Diagnostics.Debug.Assert(source != null);
            System.Diagnostics.Debug.Assert(item != null);

            Source = source;
            EnergyCost = item.Parameters.AttackSpeed;
            Target = target;

            _thrownItem = item;
            _targetList = new List<Actor>();
        }

        public RedirectMessage Validate()
        {
            foreach (Terrain tile in Target)
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
