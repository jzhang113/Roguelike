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

            MenuState mainMenu = new MenuState(new[]
            {
                new MenuButton(5, 50, "Continue", Game.LoadGame),
                new MenuButton(5, 55, "New Game", Game.NewGame),
                new MenuButton(5, 60, "  Exit  ", Game.Exit)
            });
            _states.Push(mainMenu);

            _consoles = new Dictionary<Type, ConsoleInfo>
            {
                [typeof(AnimationState)] = Game.MapConsole,
                [typeof(ApplyState)] = Game.InventoryConsole,
                [typeof(AutoexploreState)] = Game.MapConsole,
                [typeof(CharSelectState)] = Game.FullConsole,
                [typeof(DropState)] = Game.InventoryConsole,
                [typeof(EquipState)] = Game.InventoryConsole,
                [typeof(InventoryState)] = Game.InventoryConsole,
                [typeof(MenuState)] = Game.FullConsole,
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
                PopState();
                if (_states.Count == 0)
                    Game.Exit();

                Game.ForceRender();
                return null;
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
            if (_states.Count == 0)
                return;

            _states.Peek().Update();
        }

        public void Draw()
        {
            if (_states.Count == 0)
                return;

            IState current = _states.Peek();
            ConsoleInfo info = _consoles[current.GetType()];
            RLConsole console = info.Console;

            current.Draw(console);
            RLConsole.Blit(console, 0, 0, console.Width, console.Height, _rootConsole, info.X, info.Y);
        }
    }
}
