using RLNET;
using Roguelike.Commands;
using Roguelike.Utils;
using System;

namespace Roguelike.State
{
    class ApplyState : ModalState
    {
        private static readonly Lazy<ApplyState> _instance = new Lazy<ApplyState>(() => new ApplyState());
        public static ApplyState Instance => _instance.Value;

        private ApplyState()
        {
        }

        public override ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            char keyChar = keyPress.Key.ToChar();
            return new ApplyCommand(Game.Player, keyChar);
        }
    }
}