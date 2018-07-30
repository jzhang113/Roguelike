using RLNET;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.State;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    class StateHandler
    {
        private readonly RLRootConsole _rootConsole;
        private readonly Stack<IState> _states;
        private readonly IDictionary<Type, ConsoleInfo> _consoles;

        public StateHandler(RLRootConsole console)
        {
            _rootConsole = console;
            _states = new Stack<IState>();
            _states.Push(NormalState.Instance);
            _states.Push(CharSelectState.Instance);

            _consoles = new Dictionary<Type, ConsoleInfo>
            {
                [typeof(AnimationState)] = Game.MapConsole,
                [typeof(ApplyState)] = Game.InventoryConsole,
                [typeof(AutoexploreState)] = Game.MapConsole,
                [typeof(CharSelectState)] = Game.FullConsole,
                [typeof(DropState)] = Game.InventoryConsole,
                [typeof(EquipState)] = Game.InventoryConsole,
                [typeof(InventoryState)] = Game.InventoryConsole,
                [typeof(NormalState)] = Game.MapConsole,
                [typeof(TargettingState)] = Game.MapConsole,
                [typeof(TextInputState)] = Game.MapConsole,
                [typeof(UnequipState)] = Game.InventoryConsole
            };
        }

        public void Reset()
        {
            _states.Clear();
            _states.Push(NormalState.Instance);
        }

        public ICommand HandleInput()
        {
            IState currentState = _states.Peek();
            ICommand command = currentState.HandleMouseInput(_rootConsole.Mouse);
            if (command != null)
                return command;

            RLKeyPress keyPress = _rootConsole.Keyboard.GetKeyPress();
            if (keyPress?.Key == RLKey.Escape)
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
            Game.OverlayHandler.ClearBackground();
            Game.OverlayHandler.ClearForeground();
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
            IState current = _states.Peek();
            ConsoleInfo info = _consoles[current.GetType()];
            RLConsole console = info.Console;

            current.Draw(info.Console);
            RLConsole.Blit(console, 0, 0, console.Width, console.Height, _rootConsole, info.X, info.Y);
        }
    }
}
