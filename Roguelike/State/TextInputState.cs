using System;
using System.Text;
using RLNET;
using Roguelike.Commands;

namespace Roguelike.State
{
    class TextInputState : IState
    {
        private IInputCommand _inputCommand;
        private StringBuilder _inputBuffer = new StringBuilder();

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            throw new NotImplementedException();
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}
