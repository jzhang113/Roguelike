using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.State
{
    internal sealed class EquipState : ItemActionState
    {
        private static readonly Lazy<EquipState> _instance = new Lazy<EquipState>(() => new EquipState());
        public static EquipState Instance => _instance.Value;

        private EquipState()
        {
        }

        protected override ICommand ResolveInput(Item item)
        {
            if (!(item is IEquippable))
            {
                Game.MessageHandler.AddMessage($"Cannot equip {item}.");
                return null;
            }

            Item split = Game.Player.Inventory.Split(item, 1);
            IEquippable equipable = split as IEquippable;
            return new EquipCommand(Game.Player, equipable);
        }

        public override void Draw(LayerInfo layer)
        {
            base.Draw(layer);
            // highlight equippable items
            Game.Player.Inventory.DrawSelected(layer, x => x is IEquippable);
        }
    }
}