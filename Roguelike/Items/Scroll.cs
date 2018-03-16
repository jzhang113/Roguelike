using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;

namespace Roguelike.Items
{
    class Scroll : Item, IUsable
    {
        public ActionSequence ApplySkill { get; }
        
        public Scroll(string name, ActionSequence action)
        {
            Symbol = '?';
            Name = name;
            ApplySkill = action;
        }

        public void Apply(IEnumerable<Terrain> targets)
        {
            foreach (IAction action in ApplySkill.Actions)
            {
                foreach (Terrain tile in targets)
                {
                    action.Activate(Carrier, tile);
                }
            }
        }
    }
}
