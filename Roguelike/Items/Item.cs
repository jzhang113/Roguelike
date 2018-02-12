using RLNET;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Skills;
using System.Collections.Generic;

namespace Roguelike.Items
{
    public abstract class Item : Drawable
    {
        public string Name { get; set; }
        public IMaterial Material { get; set; }

        public override RLColor Color { get; set; }
        public override char Symbol { get; set; }
        public override int X { get; set; }
        public override int Y { get; set; }

        protected Actor Carrier { get; set; }
        protected IList<ISkill> Abilities { get; set; }

        protected int AttackSpeed { get; set; }
        protected int Damage { get; set; }
        protected int MeleeRange { get; set; }
        protected int ThrowRange { get; set; }

        #region virtual methods
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

        public virtual void Unequip()
        {
            Game.MessageHandler.AddMessage("You cannot take it off.", Systems.OptionHandler.MessageLevel.Normal);
        }
        #endregion

        public ISkill GetBasicAttack()
        {
            return new DamageSkill(AttackSpeed, Damage);
        }

        public ISkill GetAbility(int index)
        {
            System.Diagnostics.Debug.Assert(index < Abilities.Count);

            return Abilities[index];
        }
    }
}
