using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;
using System;
using Roguelike.Actions;
using Roguelike.Actors;

namespace Roguelike.Items
{
    [Serializable]
    class Scroll : Item, IUsable
    {
        public IAction ApplySkill { get; }

        public Scroll(string name, IAction action, RLNET.RLColor color) : base(name, Materials.Paper, color, '?')
        {
            ApplySkill = action;
        }

        public void Apply(Actor source, IEnumerable<Terrain> targets)
        {
            foreach (Terrain tile in targets)
            {
                ApplySkill.Activate(source, tile);
            }
        }
    }
}
