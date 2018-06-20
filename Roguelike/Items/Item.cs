using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;
using System;
using Roguelike.Utils;

namespace Roguelike.Items
{
    [Serializable]
    public class Item
    {
        public string Name { get; }
        public IMaterial Material { get; }
        public int Count { get; private set; }

        public Drawable DrawingComponent { get; }

        public int X
        {
            get => DrawingComponent.X;
            set => DrawingComponent.X = value;
        }

        public int Y
        {
            get => DrawingComponent.Y;
            set => DrawingComponent.Y = value;
        }

        internal int AttackSpeed { get; set; } = Constants.FULL_TURN;
        internal int Damage { get; set; } = Constants.DEFAULT_DAMAGE;
        internal float MeleeRange { get; set; } = Constants.DEFAULT_MELEE_RANGE;
        internal float ThrowRange { get; set; } = Constants.DEFAULT_THROW_RANGE;

        private readonly IList<IAction> _abilities;

        public Item(string name, IMaterial material, RLNET.RLColor color, char symbol, int count = 1)
        {
            Name = name;
            Material = material;
            Count = count;

            DrawingComponent = new Drawable
            {
                Color = color,
                Symbol = symbol
            };
            _abilities = new List<IAction>();
        }

        // copy constructor
        public Item(Item other)
        {
            Name = other.Name;
            Material = other.Material;
            Count = other.Count;

            DrawingComponent = new Drawable()
            {
                X = other.X,
                Y = other.Y,
                Color = other.DrawingComponent.Color,
                Symbol = other.DrawingComponent.Symbol
            };

            AttackSpeed = other.AttackSpeed;
            Damage = other.Damage;
            MeleeRange = other.MeleeRange;
            ThrowRange = other.ThrowRange;

            _abilities = new List<IAction>(other._abilities);
        }

        public void Add(int addCount) => Count += addCount;

        public void Remove(int removeCount) => Count = Math.Max(Count - removeCount, 0);

        public Item Split(int splitCount)
        {
            System.Diagnostics.Debug.Assert(Count >= splitCount);

            if (Count == splitCount)
            {
                Item copy = new Item(this);
                Count = 0;
                return copy;
            }

            Count -= splitCount;
            return new Item(this)
            {
                Count = splitCount
            };
        }

        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.");
        }

        public virtual void Attack()
        {
            throw new NotImplementedException();
        }

        public virtual void Throw()
        {
            throw new NotImplementedException();
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

        // Helper method for merge items. Doesn't check if the items are at the same position.
        internal bool SameAs(Item other)
        {
            return Name == other.Name &&
                Material.Equals(other.Material);
        }
    }
}
