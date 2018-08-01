using RLNET;
using Roguelike.Commands;
using Roguelike.Core;
using System.Collections.Generic;

namespace Roguelike.State
{
    class MenuState : IState
    {
        private IEnumerable<MenuButton> _buttons;

        public MenuState(IEnumerable<MenuButton> buttons)
        {
            _buttons = buttons;
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            foreach (MenuButton button in _buttons)
            {
                button.Hover = mouse.X >= button.X && mouse.X < button.X + button.Width
                    && mouse.Y >= button.Y && mouse.Y < button.Y + button.Height;

                if (button.Hover && mouse.GetLeftClick())
                    button.Callback();
            }

            return null;
        }

        public void Update()
        {
            Game.StateHandler.HandleInput();
            Game.ForceRender();
        }

        public void Draw(RLConsole console)
        {
            foreach (MenuButton button in _buttons)
            {
                button.Draw(console);
            }
        }
    }
}
