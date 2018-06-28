using RLNET;
using Roguelike.Commands;
using Roguelike.Systems;
using Roguelike.Utils;
using System;
using System.Text;

namespace Roguelike.State
{
    class TextInputState : IState
    {
        private readonly StringBuilder _inputBuffer;

        public TextInputState()
        {
            _inputBuffer = new StringBuilder();

            OverlayHandler.DisplayText = "Drop how many?";
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            switch (keyPress.Key)
            {
                case RLKey.BackSpace:
                    if (_inputBuffer.Length > 0) _inputBuffer.Length--;
                    break;
                case RLKey.Enter:
                case RLKey.KeypadEnter:
                    return OnSubmit(new InputEventArgs(_inputBuffer.ToString()));
                default:
                    _inputBuffer.Append(keyPress.Key.ToChar());
                    break;
            }

            OverlayHandler.DisplayText = $"Drop how many? {_inputBuffer}";
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

            if (EventScheduler.Execute(Game.Player, command))
                Game.StateHandler.PopState();
        }

        public void Draw()
        {
            OverlayHandler.Draw(Game.MapConsole);
            RLConsole.Blit(Game.MapConsole, 0, 0, Game.Config.MapView.Width, Game.Config.MapView.Height, Game.RootConsole, 0, Game.Config.MessageView.Height);
        }

        public event CommandEventHandler<InputEventArgs> Submit;

        protected virtual ICommand OnSubmit(InputEventArgs e)
        {
            return Submit?.Invoke(this, e);
        }

        public class InputEventArgs : EventArgs
        {
            public string Input { get; }

            public InputEventArgs(string text)
            {
                Input = text;
            }
        }
    }
}
