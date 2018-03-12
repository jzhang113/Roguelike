using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Actions
{
    class Skill
    {
        public int Speed { get; }
        public int Power { get; }
        public TargetZone Area { get; }
        
        private IEnumerable<IAction> _actions;

        public Skill(int speed, IEnumerable<IAction> actionSequence, TargetZone area)
        {
            Speed = speed;
            Area = area;
            _actions = actionSequence;
        }

        public void Activate(IEnumerable<Terrain> targets)
        {
            foreach (Terrain tile in targets)
            {
                foreach (IAction skill in _actions)
                {
                    skill.Activate(tile);
                }
            }
        }
    }
}
