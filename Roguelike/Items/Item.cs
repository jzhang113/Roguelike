﻿using RLNET;
using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;
using System;

namespace Roguelike.Items
{
    [Serializable]
    public abstract class Item : Drawable
    {
        public string Name { get; protected set; }
        public IMaterial Material { get; protected set; }
        public Actor Carrier { get; set; }

        public override int X { get; set; }
        public override int Y { get; set; }

        protected IList<IAction> Abilities { get; set; }

        protected int AttackSpeed { get; set; }
        protected int Damage { get; set; }
        protected float MeleeRange { get; set; }
        protected float ThrowRange { get; set; }

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
