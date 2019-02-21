using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Core
{
    internal class DelayAttack : ISchedulable
    {
        public int Energy { get; set; }
        public int ActivationEnergy { get; }

        public string Name => "attack";
        public int Lifetime { get; private set; } = 1;

        public IEnumerable<Loc> Targets { get; }

        private readonly ISchedulable _source;
        private readonly IAction _action;

        public DelayAttack(ISchedulable source, IAction action, IEnumerable<Loc> targets)
        {
            Energy = 0;
            ActivationEnergy = action.Speed;
            _source = source;
            _action = action;
            Targets = targets;
        }

        public ICommand Act()
        {
            Lifetime = 0;
            return new ActionCommand(_source, _action, Targets);
        }

        public int CompareTo(ISchedulable other) => Energy - other.Energy;
    }
}
