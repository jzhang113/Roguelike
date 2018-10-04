using BearLib;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.State;
using System;
using System.Collections.Generic;

namespace Roguelike.Systems
{
    public class StateHandler
    {
        private readonly Stack<IState> _states;

        private static readonly IDictionary<Type, LayerInfo> _consoles = new Dictionary<Type, LayerInfo>
        {
            [typeof(AnimationState)] = Game.MapLayer,
            [typeof(ApplyState)] = Game.InventoryLayer,
            [typeof(AutoexploreState)] = Game.MapLayer,
            [typeof(CharSelectState)] = Game.FullConsole,
            [typeof(DropState)] = Game.InventoryLayer,
            [typeof(EquipState)] = Game.InventoryLayer,
            [typeof(InventoryState)] = Game.InventoryLayer,
            [typeof(MenuState)] = Game.FullConsole,
            [typeof(NormalState)] = Game.MapLayer,
            [typeof(TargettingState)] = Game.MapLayer,
            [typeof(TextInputState)] = Game.MapLayer,
            [typeof(UnequipState)] = Game.InventoryLayer
        };

        public StateHandler()
        {
            _states = new Stack<IState>();

            MenuState mainMenu = new MenuState(new[]
            {
                new MenuButton(5, 50, "Continue", Game.LoadGame),
                new MenuButton(5, 55, "New Game", Game.NewGame),
                new MenuButton(5, 60, "  Exit  ", Game.Exit)
            });
            _states.Push(mainMenu);
        }

        public void Reset()
        {
            _states.Clear();
            _states.Push(NormalState.Instance);
        }

        public ICommand HandleInput()
        {
            // TODO: non-blocking input?
            IState currentState = _states.Peek();

            if (!Terminal.HasInput())
                return null;

            int key = Terminal.Read();
            if (key == Terminal.TK_CLOSE)
            {
                Game.Exit();
                return null;
            }

            bool left = key == Terminal.TK_MOUSE_LEFT;
            bool right = key == Terminal.TK_MOUSE_RIGHT;

            if (key == Terminal.TK_MOUSE_MOVE || left || right)
            {
                int x = Terminal.State(Terminal.TK_MOUSE_X);
                int y = Terminal.State(Terminal.TK_MOUSE_Y);

                return currentState.HandleMouseInput(x, y, left, right);
            }

            if (key == Terminal.TK_ESCAPE)
            {
                PopState();
                if (_states.Count == 0)
                    Game.Exit();

                return null;
            }

            return currentState.HandleKeyInput(key);
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
                Game.Exit();
            else
                _states.Peek().Update();
        }

        public void Draw()
        {
            if (_states.Count == 0)
                return;

            IState current = _states.Peek();
            LayerInfo info = _consoles[current.GetType()];
            Terminal.Layer(info.Z);
            current.Draw();
        }
    }
}
