using Roguelike.Commands;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    class ApplyState : ItemActionState
    {
        private static readonly Lazy<ApplyState> _instance = new Lazy<ApplyState>(() => new ApplyState());
        public static ApplyState Instance => _instance.Value;

        private ApplyState()
        {
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            ItemCount splitCount = Game.Player.Inventory.Split(new ItemCount { Item = itemCount.Item, Count = 1 });
            IUsable usableItem = splitCount.Item as IUsable;
            if (usableItem == null)
            {
                Game.MessageHandler.AddMessage($"Don't know how to apply {itemCount.Item.Name}.");
                return null;
            }

            return new ApplyCommand(Game.Player, usableItem);
        }
    }
}