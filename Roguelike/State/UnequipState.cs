using RLNET;
using Roguelike.Commands;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    class UnequipState : ModalState
    {
        private static readonly Lazy<UnequipState> _instance = new Lazy<UnequipState>(() => new UnequipState());
        public static UnequipState Instance => _instance.Value;

        private UnequipState()
        {
        }

        public override ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            char keyChar = keyPress.Key.ToChar();
            return new UnequipCommand(Game.Player, keyChar);
        }
    }
}