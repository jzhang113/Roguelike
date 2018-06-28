using Roguelike.Commands;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    class EquipState : ItemActionState
    {
        private static readonly Lazy<EquipState> _instance = new Lazy<EquipState>(() => new EquipState());
        public static EquipState Instance => _instance.Value;

        private EquipState()
        {
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            if (itemCount.Item is IEquipable)
            {
                ItemCount splitCount = Game.Player.Inventory.Split(new ItemCount {Item = itemCount.Item, Count = 1});
                IEquipable equipable = splitCount.Item as IEquipable;
                return new EquipCommand(Game.Player, equipable);
            }

            Game.MessageHandler.AddMessage($"Cannot equip {itemCount.Item}.");
            return null;
        }
    }
}