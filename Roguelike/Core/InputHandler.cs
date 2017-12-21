using RLNET;
using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class InputHandler
    {
        private static ICommand up = new MoveUpCommand();
        private static ICommand down = new MoveDownCommand();
        private static ICommand left = new MoveLeftCommand();
        private static ICommand right = new MoveRightCommand();

        public ICommand HandleInput(RLRootConsole console)
        {
            RLKeyPress keyPress = console.Keyboard.GetKeyPress();

            if (keyPress == null) return null;
            else if (keyPress.Key == RLKey.Up) return up;
            else if (keyPress.Key == RLKey.Down) return down;
            else if (keyPress.Key == RLKey.Left) return left;
            else if (keyPress.Key == RLKey.Right) return right;
            else if (keyPress.Key == RLKey.Escape)
            {
                console.Close();
                return null;
            }
            else return null;        
        }
    }
}
