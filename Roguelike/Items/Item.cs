using RLNET;
using Roguelike.Actors;
using Roguelike.Interfaces;

namespace Roguelike.Items
{
    public class Item : Drawable, IObject
    {
        public int AttackSpeed { get; set; }
        public int Damage { get; set; }
        public int MeleeRange { get; set; }
        public int ThrowRange { get; set; }

        public string Name { get; set; }
        public IMaterial Material { get; set; }
        public override RLColor Color { get; set; }
        public override char Symbol { get; set; }
        public override int X { get; set; }
        public override int Y { get; set; }
        public object Carrier { get; internal set; }

        public virtual void Apply(Actor actor)
        {
            Game.MessageHandler.AddMessage("Nothing happens.", Systems.OptionHandler.MessageLevel.Normal);
        }

        public virtual void Attack()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.", Systems.OptionHandler.MessageLevel.Normal);
        }

        public virtual void Equip(Actor actor)
        {
            Game.MessageHandler.AddMessage("You cannot equip this.", Systems.OptionHandler.MessageLevel.Normal);
        }

        public virtual void Throw()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Unequip(Actor actor)
        {
            Game.MessageHandler.AddMessage("You cannot take it off.", Systems.OptionHandler.MessageLevel.Normal);
        }
    }
}
