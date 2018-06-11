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
        public bool BlocksLight { get; }

        public Actor Carrier { get; set; }
        public override int X { get; set; }
        public override int Y { get; set; }

        internal int AttackSpeed { get; set; } = Constants.FULL_TURN;
        internal int Damage { get; set; } = Constants.DEFAULT_DAMAGE;
        internal float MeleeRange { get; set; } = Constants.DEFAULT_MELEE_RANGE;
        internal float ThrowRange { get; set; } = Constants.DEFAULT_THROW_RANGE;

        private IList<IAction> _abilities { get; }

        public Item(string name, IMaterial material, bool blocksLight = false)
        {
            Name = name;
            Material = material;
            BlocksLight = blocksLight;

            _abilities = new List<IAction>();
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

        public IAction GetBasicAttack(int targetX, int targetY)
        {
            return new DamageAction(Damage, new Core.TargetZone(Enums.TargetShape.Directional, (targetX, targetY)));
        }

        public IAction GetAbility(int index)
        {
            if (index >= _abilities.Count)
                return null;
            else
                return _abilities[index];
        }

        public void AddAbility(IAction skill)
        {
            // TODO: check that the skill doesn't already exist
            _abilities.Add(skill);
        }
    }
}
