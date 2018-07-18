using Roguelike.Commands;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    class UnequipState : ItemActionState
    {
        private static readonly Lazy<UnequipState> _instance = new Lazy<UnequipState>(() => new UnequipState());
        public static UnequipState Instance => _instance.Value;

        private UnequipState()
        {
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            if (itemCount.Item is IEquippable)
            {
                ItemCount splitCount = Game.Player.Inventory.Split(new ItemCount
                {
                    Item = itemCount.Item,
                    Count = 1
                });
                IEquippable equipable = splitCount.Item as IEquippable;
                return new UnequipCommand(Game.Player, equipable);
            }

            Game.MessageHandler.AddMessage($"Cannot unequip {itemCount.Item}.");
            return null;
        }
    }
}