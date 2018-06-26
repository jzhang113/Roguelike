using RLNET;
using Roguelike.Commands;
using System;

namespace Roguelike.State
{
    class DropState : IState
    {
        private static Lazy<DropState> _instance = new Lazy<DropState>(() => new DropState());
        public static DropState Instance => _instance.Value;

        private DropState()
        {
        }

        public void Cleanup()
        {
            throw new System.NotImplementedException();
        }

        public void Draw()
        {
            throw new System.NotImplementedException();
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            throw new System.NotImplementedException();
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }
    }
}