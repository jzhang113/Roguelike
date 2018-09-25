using Roguelike.Commands;
using Roguelike.Core;
using System.Collections.Generic;

namespace Roguelike.State
{
    internal class MenuState : IState
    {
        private IEnumerable<MenuButton> _buttons;

        public MenuState(IEnumerable<MenuButton> buttons)
        {
            _buttons = buttons;
        }

        public ICommand HandleKeyInput(int key)
        {
            return null;
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            foreach (MenuButton button in _buttons)
            {
                button.Hover = x >= button.X && x < button.X + button.Width
                    && y >= button.Y && y < button.Y + button.Height;

                if (button.Hover && leftClick)
                    button.Callback();
            }

            return null;
        }

        public void Update()
        {
            Game.StateHandler.HandleInput();
            Game.ForceRender();
        }

        public void Draw()
        {
            foreach (MenuButton button in _buttons)
            {
                button.Draw();
            }
        }
    }
}
