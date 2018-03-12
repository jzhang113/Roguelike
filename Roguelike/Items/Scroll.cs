using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;

namespace Roguelike.Items
{
    class Scroll : Item, IUsable
    {
        public Skill ApplySkill { get; }
        
        public Scroll(string name, Skill action)
        {
            Symbol = '?';
            Name = name;
            ApplySkill = action;
        }

        public void Apply(IEnumerable<Terrain> targets)
        {
            ApplySkill.Activate(targets);
        }
    }
}
