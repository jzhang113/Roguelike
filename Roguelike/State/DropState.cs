using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    internal sealed class DropState : ItemActionState
    {
        private static readonly Lazy<DropState> _instance = new Lazy<DropState>(() => new DropState());
        public static DropState Instance => _instance.Value;

        private DropState()
        {
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            if (itemCount.Count == 1)
                return new DropCommand(Game.Player, itemCount, 1);

            Game.StateHandler.PushState(new TextInputState(input =>
            {
                if (int.TryParse(input, out int dropAmount) && dropAmount > 0)
                    return new DropCommand(Game.Player, itemCount, dropAmount);

                Game.MessageHandler.AddMessage($"Unknown amount: {input}");
                return null;
            }));
            return null;
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);

            // TODO: if we have cursed items, highlight non-cursed items as droppable
            Game.Player.Inventory.DrawSelected(layer, _ => true);
        }
    }
}