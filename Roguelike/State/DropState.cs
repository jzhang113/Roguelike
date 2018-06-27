using RLNET;
using Roguelike.Commands;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    class DropState : ModalState
    {
        private static readonly Lazy<DropState> _instance = new Lazy<DropState>(() => new DropState());
        public static DropState Instance => _instance.Value;

        private DropState()
        {
        }

        public override ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            char keyChar = keyPress.Key.ToChar();
            return new DropCommand(Game.Player, keyChar);
        }
    }
}