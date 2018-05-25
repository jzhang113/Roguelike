using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;

namespace Roguelike.Items
{
    class Scroll : Item, IUsable
    {
        public IAction ApplySkill { get; }

        public Scroll(string name, IAction action)
        {
            Symbol = '?';
            Name = name;
            ApplySkill = action;
        }

        public void Apply(IEnumerable<Terrain> targets)
        {
            foreach (Terrain tile in targets)
            {
                ApplySkill.Activate(Carrier, tile);
            }
        }
    }
}
