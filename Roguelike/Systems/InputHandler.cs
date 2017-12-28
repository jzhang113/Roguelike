using RLNET;
using RogueSharp;
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
            RLMouse click = console.Mouse;
            Point square = GetClickPosition(click.X, click.Y);

            if (square != null)
            {
                Cell source = Game.Map.GetCell(Game.Player.X, Game.Player.Y);
                Cell dest = Game.Map.GetCell(square.X, square.Y);
                PathFinder pathFinder = new PathFinder(Game.Map);

                try
                {
                    if (source != dest && dest.IsExplored)
                    {
                        Path shortest = pathFinder.ShortestPath(source, dest);

                        foreach (Cell cell in Game.Map.GetAllCells())
                        {
                            Game.Map.highlight[cell.X][cell.Y] = false;
                        }

                        foreach (Cell cell in shortest.Steps)
                        {
                            if (cell.IsExplored)
                            {
                                Game.Map.highlight[cell.X][cell.Y] = true;
                            }
                        }
                    }
                }
                catch (PathNotFoundException)
                {
                    // do nothing
                }
            }

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

        private static Point GetClickPosition(int x, int y)
        {
            int mapTop = Game.Config.MessageView.Height;
            int mapBottom = Game.Config.MessageView.Height + Game.Config.MapView.Height;
            int mapLeft = 0;
            int mapRight = Game.Config.MapView.Width;
            
            if (x > mapLeft && x < mapRight && y > mapTop && y < mapBottom)
            {
                int xPos = (x - mapLeft);
                int yPos = (y - mapTop);
                return new Point(xPos, yPos);
            }
            else
            {
                return null;
            }
        }
    }
}
