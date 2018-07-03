using RLNET;
using Roguelike.Commands;
using Roguelike.Items;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    class InventoryState : ItemActionState
    {
        private static readonly Lazy<InventoryState> _instance = new Lazy<InventoryState>(() => new InventoryState());
        public static InventoryState Instance => _instance.Value;

        private InventoryState()
        {
        }

        public override ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            if (keyPress == null)
                return null;

            char keyChar = keyPress.Key.ToChar();
            bool opened = Game.Player.Inventory.OpenStack(keyChar);
            if (opened)
            {
                //Game.GameState = Mode.SubInv;
            }

            if (Game.Player.Inventory.HasKey(keyChar))
            {
                //Game.GameState = Mode.InvMenu;
            }

            // TODO
            return null;
        }

        protected override ICommand ResolveInput(ItemCount itemCount)
        {
            throw new NotImplementedException();
        }
    }
}