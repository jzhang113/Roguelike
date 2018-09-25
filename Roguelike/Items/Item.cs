using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;
using System;
using System.Drawing;

namespace Roguelike.Items
{
    [Serializable]
    public class Item
    {
        public int Enchantment { get; set; }
        public bool Burning { get; set; }

        public ItemParameter Parameters { get; }
        public Drawable DrawingComponent { get; }

        public int X { get; set; }
        public int Y { get; set; }

        internal MovesetHandler Moveset { get; set; }

        public string Name => Parameters.Name;

        public Item(ItemParameter parameters, Color color, char symbol)
        {
            Parameters = parameters;
            DrawingComponent = new Drawable(color, symbol, true);

            Moveset = new MovesetHandler(new ActionNode(
                null, null,
                new DamageAction(
                    Parameters.Damage,
                    new TargetZone(TargetShape.Range, Parameters.MeleeRange)),
                "attack"));
        }

        // copy constructor
        public Item(Item other)
        {
            Enchantment = other.Enchantment;

            Parameters = other.Parameters;
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

        public virtual Item DeepClone()
        {
            return new Item(this);
        }
        #endregion

        public IAction AttackLeft() => Moveset.ChooseLeft();

        public IAction AttackRight() => Moveset.ChooseRight();

        public void AttackReset() => Moveset.Reset();

        public override string ToString()
        {
            return $"{Enchantment:+0;-#} {Parameters.Name}";
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
    }
}
