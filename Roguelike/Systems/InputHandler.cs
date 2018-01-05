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
        public static IAction HandleInput(RLRootConsole console)
        {
            RLMouse click = console.Mouse;
            DungeonMap map = Game.Map;
            Player player = Game.Player;
            Point square = GetClickPosition(click.X, click.Y);

            if (square != null)
            {
                map.ClearHighlight();
                Cell current = map.GetCell(square.X, square.Y);
                IEnumerable<WeightedPoint> path = map.PathToPlayer(square.X, square.Y);

                if (current.IsWalkable)
                    map.Highlight[square.X, square.Y] = RLColor.Red;

                foreach (WeightedPoint p in path)
                    map.Highlight[p.X, p.Y] = RLColor.Red;

                /*
                if (click.GetLeftClick())
                {
                    foreach (WeightedPoint p in path.Reverse())
                        yield return new MoveAction(player, p.X, p.Y);

                    if (current.IsWalkable)
                        yield return new MoveAction(player, square.X, square.Y);
                    else
                        yield return new AttackAction(player, map.GetActor(current), player.BasicAttack);
                }
                */
            }

            RLKeyPress keyPress = console.Keyboard.GetKeyPress();
            if (keyPress == null)
                return null;

            switch (keyPress.Key)
            {
                case RLKey.Keypad4:
                case RLKey.H: return new MoveAction(player, player.X - 1, player.Y);
                case RLKey.Keypad2:
                case RLKey.J: return new MoveAction(player, player.X, player.Y + 1);
                case RLKey.Keypad8:
                case RLKey.K: return new MoveAction(player, player.X, player.Y - 1);
                case RLKey.Keypad6:
                case RLKey.L: return new MoveAction(player, player.X + 1, player.Y);
                case RLKey.Keypad7:
                case RLKey.Y: return new MoveAction(player, player.X - 1, player.Y - 1);
                case RLKey.Keypad9:
                case RLKey.U: return new MoveAction(player, player.X + 1, player.Y - 1);
                case RLKey.Keypad1:
                case RLKey.B: return new MoveAction(player, player.X - 1, player.Y + 1);
                case RLKey.Keypad3:
                case RLKey.N: return new MoveAction(player, player.X + 1, player.Y + 1);
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
