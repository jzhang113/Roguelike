using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Actions;
using System.Collections.Generic;
using System;

namespace Roguelike.Items
{
    [Serializable]
    public class Item
    {
        public int Count { get; private set; }

        public ItemParameters Parameters { get; }
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

        public string Name => Parameters.Name;

        private readonly IList<IAction> _abilities;

        public Item(ItemParameters parameters, RLNET.RLColor color, char symbol, int count = 1)
        {
            Count = count;
            Parameters = parameters;
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
            Count = other.Count;
            Parameters = other.Parameters;
            DrawingComponent = new Drawable()
            {
                X = other.X,
                Y = other.Y,
                Color = other.DrawingComponent.Color,
                Symbol = other.DrawingComponent.Symbol
            };
            
            _abilities = new List<IAction>(other._abilities);
        }

        public void Add(int addCount) => Count += addCount;

        public void Remove(int removeCount) => Count = Math.Max(Count - removeCount, 0);

        public Item Split(int splitCount)
        {
            System.Diagnostics.Debug.Assert(Count >= splitCount);
            Count -= splitCount;
            Item copy = DeepClone();
            copy.Count = splitCount;
            return copy;
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

        public virtual Item DeepClone()
        {
            return new Item(this);
        }
        #endregion

        public IAction GetBasicAttack(int targetX, int targetY)
        {
            return new DamageAction(Parameters.Damage, new Core.TargetZone(Enums.TargetShape.Directional, (targetX, targetY)));
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

        // Helper method for merging hard stacks.
        internal bool SameAs(Item other)
        {
            return Name == other.Name &&
                Parameters.Equals(other.Parameters);
        }

        // Helper method for merging soft stacks.
        internal bool SimilarTo(Item other)
        {
            return Name == other.Name;
        }
    }
}
