using RLNET;
using Roguelike.Commands;
using Roguelike.Utils;
using System;
using System.Text;

namespace Roguelike.State
{
    class TextInputState : IState
    {
        private readonly StringBuilder _inputBuffer;
        private readonly Func<string, ICommand> _createCommand;

        public TextInputState(Func<string, ICommand> func)
        {
            _inputBuffer = new StringBuilder();
            _createCommand = func;

            Game.OverlayHandler.DisplayText = "Drop how many?";
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            if (keyPress == null)
                return null;

            switch (keyPress.Key)
            {
                case RLKey.BackSpace:
                    if (_inputBuffer.Length > 0) _inputBuffer.Length--;
                    break;
                case RLKey.Enter:
                case RLKey.KeypadEnter:
                    return _createCommand(_inputBuffer.ToString());
                default:
                    _inputBuffer.Append(keyPress.Key.ToChar());
                    break;
            }

            Game.OverlayHandler.DisplayText = $"Drop how many? {_inputBuffer}";
            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            return null;
        }

        public void Update()
        {
            Game.ForceRender();
            ICommand command = Game.StateHandler.HandleInput();
            if (command == null)
                return;

            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
            Game.StateHandler.PopState();
        }

        public void Draw(RLConsole mapConsole)
        {
            Game.OverlayHandler.Draw(mapConsole);
        }
    }
}
