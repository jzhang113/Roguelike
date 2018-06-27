using RLNET;
using Roguelike.Commands;
using Roguelike.State;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    class StateHandler
    {
        private readonly RLRootConsole _console;
        private readonly Stack<IState> _states;

        public StateHandler(RLRootConsole console)
        {
            _console = console;
            _states = new Stack<IState>();
            _states.Push(NormalState.Instance);
        }

        public ICommand HandleInput()
        {
            IState currentState = _states.Peek();
            ICommand command = currentState.HandleMouseInput(_console.Mouse);
            if (command != null)
                return command;

            RLKeyPress keyPress = _console.Keyboard.GetKeyPress();
            if (keyPress == null)
                return null;

            if (keyPress.Key == RLKey.Escape)
            {
                switch (currentState)
                {
                    case NormalState _:
                        Game.Exit();
                        return null;
                    default:
                        PopState();
                        Game.ForceRender();
                        return null;
                }
            }

            return currentState.HandleKeyInput(keyPress);
        }

        public void PopState()
        {
            _states.Pop();
        }

        public void PushState(IState state)
        {
            _states.Push(state);
        }

        public void Update()
        {
            _states.Peek().Update();
        }

        public void Draw()
        {
            _states.Peek().Draw();
        }
    }
}
