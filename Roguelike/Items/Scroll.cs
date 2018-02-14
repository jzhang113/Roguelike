using Roguelike.Actors;
using Roguelike.Interfaces;

namespace Roguelike.Items
{
    class Scroll : Item, IUsable
    {
        public ISkill ApplyAction { get; protected set; }

        public Scroll(string name, ISkill action)
        {
            Symbol = '!';
            Name = name;
            ApplyAction = action;
        }

        public void Apply(Actor target)
        {
            ApplyAction.Activate(target);
        }
    }
}
