using System;
using RLNET;
using Roguelike.Commands;

namespace Roguelike.State
{
    class ApplyState : IState
    {
        private static Lazy<ApplyState> _instance = new Lazy<ApplyState>(() => new ApplyState());
        public static ApplyState Instance => _instance.Value;

        private ApplyState()
        {
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

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

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }
    }
}