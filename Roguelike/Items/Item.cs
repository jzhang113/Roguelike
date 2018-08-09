using MessagePack;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;

namespace Roguelike.Items
{
    [MessagePackObject]
    public class Item
    {
        [Key(0)]
        public int Enchantment { get; set; }
        [Key(1)]
        public bool Burning { get; set; }

        [Key(2)]
        public ItemParameter Parameters { get; }
        [Key(3)]
        public Drawable DrawingComponent { get; }
        
        [Key(4)]
        public int X { get; set; }
        [Key(5)]
        public int Y { get; set; }

        [Key(6)]
        private readonly IList<IAction> _abilities;

        [IgnoreMember]
        public string Name => Parameters.Name;

        // Deserialization constructor
        public Item()
        {
        }

        public Item(ItemParameter parameters, RLNET.RLColor color, char symbol)
        {
            Parameters = parameters;
            DrawingComponent = new Drawable(color, symbol, true);

            _abilities = new List<IAction>();
        }

        // copy constructor
        public Item(Item other)
        {
            Enchantment = other.Enchantment;

            Parameters = other.Parameters;
            DrawingComponent = new Drawable(
                other.DrawingComponent.Color, other.DrawingComponent.Symbol, true);
            
            _abilities = new List<IAction>(other._abilities);
        }
        
        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.");
        }

        public virtual IAction Attack()
        {
            return new DamageAction(
                Parameters.Damage, new TargetZone(TargetShape.Range, Parameters.MeleeRange));
        }

        public virtual IAction Throw()
        {
            return new DamageAction(
                Parameters.Damage, new TargetZone(TargetShape.Range, Parameters.ThrowRange));
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
