using Roguelike.Commands;
using Roguelike.Core;
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
            if (itemCount.Item is IEquippable)
            {
                ItemCount splitCount = Game.Player.Inventory.Split(new ItemCount
                {
                    Item = itemCount.Item,
                    Count = 1
                });
                IEquippable equipable = splitCount.Item as IEquippable;
                return new EquipCommand(Game.Player, equipable);
            }

            Game.MessageHandler.AddMessage($"Cannot equip {itemCount.Item}.");
            return null;
        }
    }
}