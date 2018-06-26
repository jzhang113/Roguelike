using RLNET;
using Roguelike.Commands;
using System;

namespace Roguelike.State
{
    class InventoryState : IState
    {
        private static Lazy<InventoryState> _instance = new Lazy<InventoryState>(() => new InventoryState());
        public static InventoryState Instance => _instance.Value;

        private InventoryState()
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