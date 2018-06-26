using RLNET;
using Roguelike.Commands;
using System;

namespace Roguelike.State
{
    class EquipState : IState
    {
        private static Lazy<EquipState> _instance = new Lazy<EquipState>(() => new EquipState());
        public static EquipState Instance => _instance.Value;

        private EquipState()
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