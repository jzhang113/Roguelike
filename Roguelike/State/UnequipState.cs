using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    internal sealed class UnequipState : ItemActionState
    {
        private static readonly Lazy<UnequipState> _instance = new Lazy<UnequipState>(() => new UnequipState());
        public static UnequipState Instance => _instance.Value;

        private UnequipState()
        {
        }

        protected override ICommand ResolveInput(Item item)
        {
            if (item is IEquippable)
            {
                Item split = Game.Player.Inventory.Split(item, 1);
                IEquippable equipable = split as IEquippable;
                return new UnequipCommand(Game.Player, equipable);
            }

            Game.MessageHandler.AddMessage($"Cannot unequip {item}.");
            return null;
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);
            layer.Print(-1, $"{Constants.HEADER_LEFT}[color=grass]INVENTORY[/color]" +
                $"[color=white]{Constants.HEADER_SEP}EQUIPMENT[/color]{Constants.HEADER_RIGHT}");
        }
    }
}