using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using Roguelike.Actions;

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

        protected Scroll(SerializationInfo info, StreamingContext context) : base(info, context)
        {
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
