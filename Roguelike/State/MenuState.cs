using Optional;
using Roguelike.Commands;
using Roguelike.Core;
using System.Collections.Generic;

namespace Roguelike.State
{
    internal class MenuState : IState
    {
        private readonly IEnumerable<MenuButton> _buttons;

        public MenuState(IEnumerable<MenuButton> buttons)
        {
            _buttons = buttons;
        }

        public Option<ICommand> HandleKeyInput(int key)
        {
            return Option.None<ICommand>();
        }

        public Option<ICommand> HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            foreach (MenuButton button in _buttons)
            {
                button.Hover = button.Position.Contains(x, y);
                if (button.Hover && leftClick)
                    button.Callback();
            }

            return Option.None<ICommand>();
        }

        public void Update(ICommand command)
        {
            // TODO: do menu things
        }

        public void Draw(LayerInfo layer)
        {
            foreach (MenuButton button in _buttons)
            {
                button.Draw(layer);
            }
        }
    }
}
