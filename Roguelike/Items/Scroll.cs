using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Skills;
using System;

namespace Roguelike.Items
{
    class Scroll : Item, IUsable
    {
        public Skill ApplySkill { get; private set;}

        private TargetZone _area;

        public Scroll(string name, Skill action, TargetZone area)
        {
            Symbol = '!';
            Name = name;
            ApplySkill = action;
            _area = area;
        }

        public void Apply()
        {
            ApplySkill.Activate(_area.GetTilesInRange(Carrier, null));
            // TODO: add a targetting system
        }
    }
}
