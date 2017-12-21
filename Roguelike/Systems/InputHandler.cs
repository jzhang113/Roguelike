using RLNET;
using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class InputHandler
    {
        private static ICommand moveN = new MoveCommand(0, -1);
        private static ICommand moveNE = new MoveCommand(1, -1);
        private static ICommand moveE = new MoveCommand(1, 0);
        private static ICommand moveSE = new MoveCommand(1, 1);
        private static ICommand moveS = new MoveCommand(0, 1);
        private static ICommand moveSW = new MoveCommand(-1, 1);
        private static ICommand moveW = new MoveCommand(-1, 0);
        private static ICommand moveNW = new MoveCommand(-1, -1);

        public ICommand HandleInput(RLRootConsole console)
        {
            RLKeyPress keyPress = console.Keyboard.GetKeyPress();

            if (keyPress == null) return null;

            switch (keyPress.Key)
            {
                case RLKey.H: return moveW;
                case RLKey.J: return moveS;
                case RLKey.K: return moveN;
                case RLKey.L: return moveE;
                case RLKey.Y: return moveNW;
                case RLKey.U: return moveNE;
                case RLKey.B: return moveSW;
                case RLKey.N: return moveSE;
                case RLKey.Escape:
                    console.Close();
                    return null;
                default: return null;
            }
        }
    }
}
