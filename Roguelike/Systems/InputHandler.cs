﻿using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Systems
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

        public static ICommand HandleInput(RLRootConsole console)
        {
            RLKeyPress keyPress = console.Keyboard.GetKeyPress();

            if (keyPress == null) return null;

            switch (keyPress.Key)
            {
                case RLKey.Keypad4:
                case RLKey.H: return moveW;
                case RLKey.Keypad2:
                case RLKey.J: return moveS;
                case RLKey.Keypad8:
                case RLKey.K: return moveN;
                case RLKey.Keypad6:
                case RLKey.L: return moveE;
                case RLKey.Keypad7:
                case RLKey.Y: return moveNW;
                case RLKey.Keypad9:
                case RLKey.U: return moveNE;
                case RLKey.Keypad1:
                case RLKey.B: return moveSW;
                case RLKey.Keypad3:
                case RLKey.N: return moveSE;
                case RLKey.Escape:
                    console.Close();
                    return null;
                default: return null;
            }
        }
    }
}
