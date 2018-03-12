using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Skills
{
    public class Skill
    {
        public int Speed { get; }
        public int Power { get; }
        
        private IEnumerable<IAction> _actions;

        public Skill(int speed, IEnumerable<IAction> actionSequence)
        {
            Speed = speed;
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
