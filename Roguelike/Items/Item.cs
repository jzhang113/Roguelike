using RLNET;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Skills;
using System.Collections.Generic;

namespace Roguelike.Items
{
    public abstract class Item : Drawable
    {
        public string Name { get; protected set; }
        public IMaterial Material { get; protected set; }
        public Actor Carrier { get; set; }

        public override RLColor Color { get; set; }
        public override char Symbol { get; set; }
        public override int X { get; set; }
        public override int Y { get; set; }

        protected IList<ISkill> Abilities { get; set; }

        protected int AttackSpeed { get; set; }
        protected int Damage { get; set; }
        protected int MeleeRange { get; set; }
        protected int ThrowRange { get; set; }

        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.", Systems.OptionHandler.MessageLevel.Normal);
        }

        public virtual void Attack()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Throw()
        {
            throw new System.NotImplementedException();
        }
        #endregion

        public ISkill GetBasicAttack()
        {
            return new DamageSkill(Carrier, AttackSpeed, Damage);
        }

        public ISkill GetAbility(int index)
        {
            System.Diagnostics.Debug.Assert(index < Abilities.Count);

            return Abilities[index];
        }
    }
}
