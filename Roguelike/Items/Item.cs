using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using Roguelike.Utils;
using System;
using System.Drawing;

namespace Roguelike.Items
{
    [Serializable]
    public class Item
    {
        public int Enchantment { get; set; }
        public bool Burning { get; set; } // TODO: do something with this
        public int Count { get; private set; }

        public ItemParameter Parameters { get; }
        public Drawable DrawingComponent { get; }

        public Loc Loc { get; set; }

        internal MovesetHandler Moveset { get; set; }

        public string Name => Parameters.Name;

        public Item(ItemParameter parameters, Color color, char symbol, Loc loc, int count = 1)
        {
            Parameters = parameters;
            DrawingComponent = new Drawable(color, symbol, true);
            Loc = loc;
            Count = count;

            Moveset = new MovesetHandler(new ActionNode(
                null, null,
                new DamageAction(
                    Parameters.Damage,
                    new TargetZone(TargetShape.Range, Parameters.MeleeRange)),
                "attack"));
        }

        // copy constructor
        public Item(Item other, int count)
        {
            Enchantment = other.Enchantment;
            Count = count;

            Parameters = other.Parameters;
            Moveset = other.Moveset;
            DrawingComponent = new Drawable(
                other.DrawingComponent.Color, other.DrawingComponent.Symbol, true);
        }

        #region virtual methods
        public virtual void Consume(Actor actor)
        {
            Game.MessageHandler.AddMessage("That would be unhealthy.");
        }

        public virtual IAction Throw()
        {
            return new DamageAction(
                Parameters.Damage, new TargetZone(TargetShape.Range, Parameters.ThrowRange));
        }

        public virtual Item Clone(int count)
        {
            return new Item(this, count);
        }
        #endregion

        public IAction AttackLeft() => Moveset.ChooseLeft();

        public IAction AttackRight() => Moveset.ChooseRight();

        public void AttackReset() => Moveset.Reset();

        public void Merge(Item other)
        {
            if (!SameAs(other))
                return;

            Count += other.Count;
            other.Count = 0;
        }

        public Item Split(int count)
        {
            if (count > Count)
                count = Count;

            Count -= count;
            return Clone(count);
        }

        // Helper method for merging hard stacks.
        internal bool SameAs(Item other)
        {
            return Enchantment == other.Enchantment
                   && Parameters.Equals(other.Parameters);
        }

        // Helper method for merging soft stacks.
        internal bool SimilarTo(Item other)
        {
            return Name == other.Name;
        }

        public override string ToString()
        {
            // TODO: implement identification
            string name = $"{Enchantment:+0;-#} {Parameters.Name}";
            return (Count == 1) ? name : $"{Count} {name.Pluralize()}";
        }
    }
}
