using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Utils;
using System;
using System.Text;

namespace Roguelike.State
{
    internal class TextInputState : IState
    {
        public bool Nonblocking => false;

        private readonly StringBuilder _inputBuffer;
        private readonly Func<string, ICommand> _createCommand;

        public TextInputState(Func<string, ICommand> func)
        {
            _inputBuffer = new StringBuilder();
            _createCommand = func;

            Game.Overlay.DisplayText = "Drop how many?";
        }

        public ICommand HandleKeyInput(int key)
        {
            // TODO: replace with read_str
            switch (key)
            {
                case Terminal.TK_BACKSPACE:
                    if (_inputBuffer.Length > 0) _inputBuffer.Length--;
                    break;
                case Terminal.TK_ENTER:
                case Terminal.TK_KP_ENTER:
                    return _createCommand(_inputBuffer.ToString());
                default:
                    _inputBuffer.Append(key.ToChar());
                    break;
            }

            Game.Overlay.DisplayText = $"Drop how many? {_inputBuffer}";
            return null;
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            return null;
        }

        public void Update(ICommand command)
        {
            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
            Game.StateHandler.PopState();
        }

        public void Draw(LayerInfo layer)
        {
            Game.Overlay.Draw(layer);
        }
    }
}
