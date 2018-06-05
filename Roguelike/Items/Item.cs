using RLNET;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;
using System;
using Roguelike.Utils;

namespace Roguelike.Items
{
    [Serializable]
    public abstract class Item : Drawable
    {
        public string Name { get; }
        public IMaterial Material { get; }

        public Actor Carrier { get; set; }
        public override int X { get; set; }
        public override int Y { get; set; }

        internal int AttackSpeed { get; set; } = Constants.FULL_TURN;
        internal int Damage { get; set; } = Constants.DEFAULT_DAMAGE;
        internal float MeleeRange { get; set; } = Constants.DEFAULT_MELEE_RANGE;
        internal float ThrowRange { get; set; } = Constants.DEFAULT_THROW_RANGE;

        private IList<IAction> Abilities { get; }

        public Item(string name, IMaterial material)
        {
            Name = name;
            Material = material;
            Abilities = new List<IAction>();
        }

        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.");
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

        public IAction GetBasicAttack((int X, int Y) target)
        {
            return new DamageAction(Damage, new Core.TargetZone(Enums.TargetShape.Directional, target));
        }

        public IAction GetAbility(int index)
        {
            if (index >= Abilities.Count)
                return null;
            else
                return Abilities[index];
        }

        public void AddAbility(IAction skill)
        {
            // TODO: check that the skill doesn't already exist
            Abilities.Add(skill);
        }
    }
}
