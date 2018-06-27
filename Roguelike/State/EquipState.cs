using System;
using RLNET;
using Roguelike.Commands;
using Roguelike.Utils;

namespace Roguelike.State
{
    class EquipState : ModalState
    {
        private static readonly Lazy<EquipState> _instance = new Lazy<EquipState>(() => new EquipState());
        public static EquipState Instance => _instance.Value;

        private EquipState()
        {
        }

        public override ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            char keyChar = keyPress.Key.ToChar();
            return new EquipCommand(Game.Player, keyChar);
        }
    }
}