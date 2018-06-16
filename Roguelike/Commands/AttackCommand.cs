using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Systems;
using System.Collections.Generic;
using Roguelike.Actions;

namespace Roguelike.Commands
{
    class AttackCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        private readonly IAction _skill;
        private readonly IEnumerable<Terrain> _targets;

        public AttackCommand(Actor source, IAction attack)
        {
            _skill = attack;
            Source = source;
            EnergyCost = attack.Speed;
        }

        public AttackCommand(Actor source, IAction attack, Terrain target)
        {
            _skill = attack;
            _targets = new List<Terrain>
            {
                target
            };
            Source = source;
        }

        public AttackCommand(Actor source, IAction attack, IEnumerable<Terrain> targets)
        {
            _skill = attack;
            _targets = targets;
            Source = source;
        }

        public RedirectMessage Validate()
        {
            System.Diagnostics.Debug.Assert(_skill != null);
            if (_skill == null)
                return new RedirectMessage(false);

            if (_skill.Area.Aimed && _skill.Area.Target == null)
                System.Diagnostics.Debug.Assert(_targets != null);

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            if (_targets != null)
            {
                foreach (Terrain tile in _targets)
                {
                    _skill.Activate(Source, tile);
                }
            }
            else
            {
                foreach (Terrain tile in _skill.Area.GetTilesInRange(Source))
                {
                    _skill.Activate(Source, tile);
                }
            }
        }
    }
}
