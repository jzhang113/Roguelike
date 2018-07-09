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
        public int Enchantment { get; set; }

        public ItemParameter Parameters { get; }
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

        public Item(ItemParameter parameters, RLNET.RLColor color, char symbol)
        {
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
            Enchantment = other.Enchantment;

            Parameters = other.Parameters;
            DrawingComponent = new Drawable
            {
                X = other.X,
                Y = other.Y,
                Color = other.DrawingComponent.Color,
                Symbol = other.DrawingComponent.Symbol
            };
            
            _abilities = new List<IAction>(other._abilities);
        }
        
        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.");
        }

        public virtual IAction Attack()
        {
            return new DamageAction(Parameters.Damage, new Core.TargetZone(Enums.TargetShape.Range, Parameters.MeleeRange));
        }

        public virtual IAction Throw()
        {
            return new DamageAction(Parameters.Damage, new Core.TargetZone(Enums.TargetShape.Range, Parameters.ThrowRange));
        }

        public virtual Item DeepClone()
        {
            return new Item(this);
        }
        #endregion

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

        public override string ToString()
        {
            return $"{Enchantment:+0;-#} {Parameters.Name}";
        }

        // Helper method for merging hard stacks.
        internal bool SameAs(Item other)
        {
            return Enchantment == other.Enchantment &&
                   Parameters.Equals(other.Parameters);
        }

        // Helper method for merging soft stacks.
        internal bool SimilarTo(Item other)
        {
            return Name == other.Name;
        }
    }
}
