using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Roguelike.Items
{
    [Serializable]
    public class Scroll : Item, IUsable
    {
        public IAction ApplySkill { get; }

        public Scroll(ItemParameter parameters, IAction action, Color color) : base(parameters, color, '?')
        {
            ApplySkill = action;
        }

        public Scroll(Scroll other, int count) : base(other, count)
        {
            ApplySkill = other.ApplySkill;
        }

        public void Apply(Actor source, IEnumerable<Loc> targets)
        {
            foreach (Loc point in targets)
            {
                ApplySkill.Activate(source, Game.Map.Field[point]);
            }
        }

        public override Item Clone(int count)
        {
            return new Scroll(this, count);
        }
    }
}
