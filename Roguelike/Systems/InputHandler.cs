using RLNET;
using RogueSharp;
using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Systems
{
    class InputHandler
    {
        public static ICommand HandleInput(RLRootConsole console)
        {
            RLMouse click = console.Mouse;
            DungeonMap map = Game.Map;
            Point square = GetClickPosition(click.X, click.Y);

            if (square != null)
            {
                map.ClearHighlight();
                Cell current = map.GetCell(square.X, square.Y);
                IEnumerable<Point> path = map.PathToPlayer(square.X, square.Y);

                if (current.IsWalkable)
                    Game.Map.Highlight[square.X, square.Y] = true;

                foreach (Point p in path)
                {
                    Game.Map.Highlight[p.X, p.Y] = true;                    
                }

                if (click.GetLeftClick())
                {
                    System.Console.WriteLine(map.PlayerDistance[square.X, square.Y]);

                    foreach (Point p in path.Reverse())
                    {
                        Game.EventScheduler.Schedule(new MoveAction(Game.Player, p.X, p.Y));
                    }

                    if (current.IsWalkable)
                        Game.EventScheduler.Schedule(new MoveAction(Game.Player, square.X, square.Y));
                    else
                        Game.EventScheduler.Schedule(new AttackAction(Game.Player, map.GetActor(current), Game.Player.BasicAttack));
                }
            }

            RLKeyPress keyPress = console.Keyboard.GetKeyPress();
            if (keyPress == null) return null;

            switch (keyPress.Key)
            {
                case RLKey.Keypad4:
                case RLKey.H: return Move.MoveW;
                case RLKey.Keypad2:
                case RLKey.J: return Move.MoveS;
                case RLKey.Keypad8:
                case RLKey.K: return Move.MoveN;
                case RLKey.Keypad6:
                case RLKey.L: return Move.MoveE;
                case RLKey.Keypad7:
                case RLKey.Y: return Move.MoveNW;
                case RLKey.Keypad9:
                case RLKey.U: return Move.MoveNE;
                case RLKey.Keypad1:
                case RLKey.B: return Move.MoveSW;
                case RLKey.Keypad3:
                case RLKey.N: return Move.MoveSE;
                case RLKey.Escape:
                    Game.Exit();
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
            
            if (x > mapLeft && x < mapRight - 1 && y > mapTop && y < mapBottom - 1)
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
