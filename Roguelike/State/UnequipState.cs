using RLNET;
using Roguelike.Commands;
using System;

namespace Roguelike.State
{
    class UnequipState : IState
    {
        private static Lazy<UnequipState> _instance = new Lazy<UnequipState>(() => new UnequipState());
        public static UnequipState Instance => _instance.Value;

        private UnequipState()
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