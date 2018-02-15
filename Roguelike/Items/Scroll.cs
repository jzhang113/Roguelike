using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Skills;
using System;

namespace Roguelike.Items
{
    class Scroll : Item, IUsable
    {
        public Skill ApplySkill { get; private set;}

        private Func<Actor, Actor, bool> _predicate;
        private int _targetLimit;

        public Scroll(string name, Skill action, Func<Actor, Actor, bool> predicate, int targetLimit)
        {
            Symbol = '!';
            Name = name;
            ApplySkill = action;
            _predicate = predicate;
            _targetLimit = targetLimit;
        }

        public void Apply()
        {
            ApplySkill.Source = Carrier;
            ApplySkill.ApplyTargets(_predicate, _targetLimit);
        }
    }
}
