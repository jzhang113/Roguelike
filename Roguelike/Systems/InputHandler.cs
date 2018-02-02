using RLNET;
using RogueSharp;
using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Actors;
using Roguelike.Actions;

namespace Roguelike.Systems
{
    class InputHandler
    {
        private static RLRootConsole _console;

        public static void Initialize(RLRootConsole console)
        {
            _console = console;
        }

        public static RLKeyPress GetKeyPress()
        {
            RLKeyPress keyPress = null;

            while (keyPress == null)
                keyPress = _console.Keyboard.GetKeyPress();

            return keyPress;
        }

        public static IAction HandleInput()
        {
            RLMouse click = _console.Mouse;
            DungeonMap map = Game.Map;
            Player player = Game.Player;
            Point square = GetClickPosition(click.X, click.Y);

            if (square != null)
            {
                map.ClearHighlight();
                Cell current = map.GetCell(square.X, square.Y);
                IEnumerable<WeightedPoint> path = map.PathToPlayer(square.X, square.Y).Reverse();
                bool exploredPathExists = false;

                foreach (WeightedPoint p in path)
                {
                    if (!exploredPathExists)
                        exploredPathExists = true;

                    if (!map.GetCell(p.X, p.Y).IsExplored)
                    {
                        exploredPathExists = false;
                        break;
                    }

                    map.Highlight[p.X, p.Y] = RLColor.Red;
                }

                if (current.IsWalkable && exploredPathExists)
                    map.Highlight[square.X, square.Y] = RLColor.Red;
                /*
                if (click.GetLeftClick())
                {
                    foreach (WeightedPoint p in path)
                        yield return new MoveAction(player, p.X, p.Y);

                    if (current.IsWalkable)
                        yield return new MoveAction(player, square.X, square.Y);
                    else
                        yield return new AttackAction(player, map.GetActor(current), player.BasicAttack);
                }
                */

                LookHandler.Display(map.GetActor(current), map.Field[square.X, square.Y]);
            }

            RLKeyPress keyPress = _console.Keyboard.GetKeyPress();
            if (keyPress == null)
                return null;

            if (Game.GameMode == Game.Mode.Inventory)
                return HandleInventoryInput(keyPress);

            switch (keyPress.Key)
            {
                #region Movement Keys
                case RLKey.Left:
                case RLKey.Keypad4:
                case RLKey.H: return new MoveAction(player, player.X + Move.W.X, player.Y);
                case RLKey.Down:
                case RLKey.Keypad2:
                case RLKey.J: return new MoveAction(player, player.X, player.Y + Move.S.Y);
                case RLKey.Up:
                case RLKey.Keypad8:
                case RLKey.K: return new MoveAction(player, player.X, player.Y + Move.N.Y);
                case RLKey.Right:
                case RLKey.Keypad6:
                case RLKey.L: return new MoveAction(player, player.X + Move.E.X, player.Y);
                case RLKey.Keypad7:
                case RLKey.Y: return new MoveAction(player, player.X + Move.NW.X, player.Y + Move.NW.Y);
                case RLKey.Keypad9:
                case RLKey.U: return new MoveAction(player, player.X + Move.NE.X, player.Y + Move.NE.Y);
                case RLKey.Keypad1:
                case RLKey.B: return new MoveAction(player, player.X + Move.SW.X, player.Y + Move.SW.Y);
                case RLKey.Keypad3:
                case RLKey.N: return new MoveAction(player, player.X + Move.SE.X, player.Y + Move.SE.Y);
                case RLKey.Keypad5:
                case RLKey.Period: return new MoveAction(player, player.X, player.Y);
                case RLKey.Comma: return new PickupAction(player, Game.Map.Field[player.X, player.Y].ItemStack);
                #endregion
                case RLKey.D:
                    return new DropAction(player);
                case RLKey.I:
                    Game.GameMode = Game.Mode.Inventory;
                    return null;
                case RLKey.Escape:
                    Game.Exit();
                    return null;
                default: return null;
            }
        }

        private static IAction HandleInventoryInput(RLKeyPress keyPress)
        {
            // TODO: Implement this
            switch(keyPress.Key)
            {
                case RLKey.Escape:
                    Game.GameMode = Game.Mode.Normal;
                    Game.ForceRender();
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
