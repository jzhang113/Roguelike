using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;
using System;

namespace Roguelike.Items
{
    [Serializable]
    class Scroll : Item, IUsable
    {
        public IAction ApplySkill { get; }

        public Scroll(string name, IAction action) : base(name, Materials.Paper)
        {
            DrawingComponent.Symbol = '?';
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
