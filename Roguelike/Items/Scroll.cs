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

        public Scroll(ItemParameters parameters, IAction action, RLNET.RLColor color) : base(parameters, color, '?')
        {
            ApplySkill = action;
        }

        public Scroll(Scroll other) : base(other)
        {
            ApplySkill = other.ApplySkill;
        }

        public void Apply(Actor source, IEnumerable<Terrain> targets)
        {
            foreach (Terrain tile in targets)
            {
                ApplySkill.Activate(source, tile);
            }
        }

        public override Item DeepClone()
        {
            return new Scroll(this);
        }
    }
}
