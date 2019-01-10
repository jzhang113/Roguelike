using BearLib;
using Optional;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.State;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    public class StateHandler
    {
        public LayerInfo CurrentLayer => _consoles[_states.Peek().GetType()];

        private readonly Stack<IState> _states;
        private readonly IDictionary<Type, LayerInfo> _consoles;

        public StateHandler(IDictionary<Type, LayerInfo> consoleMapping)
        {
            _states = new Stack<IState>();
            _consoles = consoleMapping;

            MenuState mainMenu = new MenuState(new[]
            {
                new MenuButton(5, Constants.SCREEN_HEIGHT - 20, "Continue", Game.LoadGame),
                new MenuButton(5, Constants.SCREEN_HEIGHT - 15, "New Game", Game.NewGame),
                new MenuButton(5, Constants.SCREEN_HEIGHT - 10, "  Exit  ", Game.Exit)
            });
            _states.Push(mainMenu);
        }

        public void Reset()
        {
            _states.Clear();
            _states.Push(NormalState.Instance);
        }

        private Option<ICommand> HandleInput()
        {
            IState currentState = _states.Peek();
            if (!Terminal.HasInput())
            {
                return (currentState is AutoexploreState)
                   ? currentState.HandleKeyInput(Terminal.TK_INPUT_NONE)
                   : Option.None<ICommand>();
            }

            int key = Terminal.Read();
            if (key == Terminal.TK_CLOSE)
            {
                Game.Exit();
                return Option.None<ICommand>();
            }

            bool left = key == Terminal.TK_MOUSE_LEFT;
            bool right = key == Terminal.TK_MOUSE_RIGHT;

            if (key == Terminal.TK_MOUSE_MOVE || left || right)
            {
                int x = Terminal.State(Terminal.TK_MOUSE_X);
                int y = Terminal.State(Terminal.TK_MOUSE_Y);
                LayerInfo layer = _consoles[currentState.GetType()];

                return layer.PointInside(x, y)
                    ? currentState.HandleMouseInput(x - layer.X, y - layer.Y, left, right)
                    : Option.None<ICommand>();
            }

            if (key == Terminal.TK_ESCAPE)
            {
                PopState();
                if (_states.Count == 0)
                    Game.Exit();

                return Option.None<ICommand>();
            }

            return currentState.HandleKeyInput(key);
        }

        public void PopState()
        {
            Game.Overlay.Clear();
            _states.Pop();
        }

        public void PushState(IState state)
        {
            _states.Push(state);
        }

        public void Update()
        {
            if (_states.Count == 0)
            {
                Game.Exit();
                return;
            }

            IState currentState = _states.Peek();
            HandleInput().MatchSome(command => currentState.Update(command));
        }

        public void Draw()
        {
            if (_states.Count == 0)
                return;

            IState current = _states.Peek();
            LayerInfo info = _consoles[current.GetType()];
            Terminal.Layer(info.Z);
            current.Draw(info);
        }
    }
}
