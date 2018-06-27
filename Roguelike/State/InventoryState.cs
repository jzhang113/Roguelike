using RLNET;
using Roguelike.Commands;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    class InventoryState : ModalState
    {
        private static readonly Lazy<InventoryState> _instance = new Lazy<InventoryState>(() => new InventoryState());
        public static InventoryState Instance => _instance.Value;

        private InventoryState()
        {
        }

        public override ICommand HandleKeyInput(RLKeyPress keyPress)
        {
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
    }
}