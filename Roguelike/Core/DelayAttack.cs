using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Core
{
    internal class DelayAttack : ISchedulable
    {
        public int Energy { get; set; }
        public int RefreshRate { get; }

        public string Name => "attack";
        public int Lifetime { get; private set; } = 1;

        private readonly ISchedulable _source;
        private readonly IAction _action;
        private readonly IEnumerable<Tile> _targets;

        public DelayAttack(ISchedulable source, IAction action, IEnumerable<Tile> targets)
        {
            Energy = 0;
            RefreshRate = action.Speed;
            _source = source;
            _action = action;
            _targets = targets;
        }

        public ICommand Act()
        {
            Lifetime = 0;
            return new ActionCommand(_source, _action, _targets);
        }

        public int CompareTo(ISchedulable other) => Energy - other.Energy;
    }
}
