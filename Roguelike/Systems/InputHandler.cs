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
        private static int _holdTimeout = 0;
        private static bool _holdingKey = false;
        private static readonly int HOLD_LIMIT = 15;

        public static void Initialize(RLRootConsole console)
        {
            _console = console;
        }

        public static ICommand HandleInput()
        {
            RLMouse click = _console.Mouse;
            MapHandler map = Game.Map;
            Player player = Game.Player;
            Point square = GetClickPosition(click.X, click.Y);

            if (square != null)
            {
                map.ClearHighlight();
                Cell current = map.GetCell(square.X, square.Y);
                IEnumerable<WeightedPoint> path = map.PathToPlayer(square.X, square.Y).Reverse();
                bool exploredPathExists = false;

                // TODO: Path may end up broken because an enemy is in the way.
                foreach (WeightedPoint p in path)
                {
                    if (!exploredPathExists)
                        exploredPathExists = true;

                    if (!map.Field[p.X, p.Y].IsExplored)
                    {
                        exploredPathExists = false;
                        break;
                    }

                    map.Highlight[p.X][p.Y] = RLColor.Red;
                }

                if (current.IsWalkable && exploredPathExists)
                    map.Highlight[square.X][square.Y] = RLColor.Red;
                /*
                if (click.GetLeftClick())
                {
                    foreach (WeightedPoint p in path)
                        yield return new MoveCommand(player, p.X, p.Y);

                    if (current.IsWalkable)
                        yield return new MoveCommand(player, square.X, square.Y);
                    else
                        yield return new AttackCommand(player, map.GetActor(current), player.BasicAttack);
                }
                */
                
                LookHandler.DisplayActor(map.GetActor(square.X, square.Y));
                LookHandler.DisplayItem(map.GetItem(square.X, square.Y));
                LookHandler.DisplayTerrain(map.Field[square.X, square.Y]);
            }
            
            RLKeyPress keyPress = _console.Keyboard.GetKeyPress();
            if (keyPress == null)
            {
                // For some reason, holding a key issues a command, but follows up with nulls.
                // We resolve this by making holds somewhat sticky.
                if (_holdingKey)
                {
                    if (_holdTimeout < HOLD_LIMIT)
                    {
                        _holdTimeout++;
                    }
                    else
                    {
                        Game.ShowOverlay = false;
                        _holdingKey = false;
                        _holdTimeout = 0;
                    }
                }

                return null;
            }

            if (Game.ShowInventory)
            {
                if (!HandleMenuInput(keyPress))
                    return null;

                char keyChar = ToChar(keyPress.Key);

                switch (Game.GameMode)
                {
                    case Game.Mode.Inventory:
                        // TODO: implement inventory actions
                        return null;
                    case Game.Mode.Apply:
                        return new ApplyCommand(player, keyChar);
                    case Game.Mode.Drop:
                        return new DropCommand(player, keyChar);
                    case Game.Mode.Equip:
                        return new EquipCommand(player, keyChar);
                    case Game.Mode.Unequip:
                        return new UnequipCommand(player, keyChar);
                }
            }

            Game.ShowOverlay = (keyPress.Key == RLKey.Tab);
            _holdingKey = true;

            if (keyPress.Shift)
            {

                switch (keyPress.Key)
                {
                    #region Movement Keys
                    case RLKey.Left:
                    case RLKey.Keypad4:
                    case RLKey.H:
                        return new AttackCommand(player, Game.Map.Field[player.X + Direction.W.X, player.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Down:
                    case RLKey.Keypad2:
                    case RLKey.J:
                        return new AttackCommand(player, Game.Map.Field[player.X, player.Y + Direction.S.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Up:
                    case RLKey.Keypad8:
                    case RLKey.K:
                        return new AttackCommand(player, Game.Map.Field[player.X, player.Y + Direction.N.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Right:
                    case RLKey.Keypad6:
                    case RLKey.L:
                        return new AttackCommand(player, Game.Map.Field[player.X + Direction.E.X, player.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Keypad7:
                    case RLKey.Y:
                        return new AttackCommand(player, Game.Map.Field[player.X + Direction.NW.X, player.Y + Direction.NW.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Keypad9:
                    case RLKey.U:
                        return new AttackCommand(player, Game.Map.Field[player.X + Direction.NE.X, player.Y + Direction.NE.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Keypad1:
                    case RLKey.B:
                        return new AttackCommand(player, Game.Map.Field[player.X + Direction.SW.X, player.Y + Direction.SW.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Keypad3:
                    case RLKey.N:
                        return new AttackCommand(player, Game.Map.Field[player.X + Direction.SE.X, player.Y + Direction.SE.Y], player.Equipment.PrimaryWeapon.GetAbility(0));
                    case RLKey.Keypad5:
                    case RLKey.Period:
                        return new WaitCommand(player);
                    #endregion
                    default: return null;
                }
            }

            switch (keyPress.Key)
            {
                #region Movement Keys
                case RLKey.Left:
                case RLKey.Keypad4:
                case RLKey.H:
                    return new MoveCommand(player, player.X + Direction.W.X, player.Y);
                case RLKey.Down:
                case RLKey.Keypad2:
                case RLKey.J:
                    return new MoveCommand(player, player.X, player.Y + Direction.S.Y);
                case RLKey.Up:
                case RLKey.Keypad8:
                case RLKey.K:
                    return new MoveCommand(player, player.X, player.Y + Direction.N.Y);
                case RLKey.Right:
                case RLKey.Keypad6:
                case RLKey.L:
                    return new MoveCommand(player, player.X + Direction.E.X, player.Y);
                case RLKey.Keypad7:
                case RLKey.Y:
                    return new MoveCommand(player, player.X + Direction.NW.X, player.Y + Direction.NW.Y);
                case RLKey.Keypad9:
                case RLKey.U:
                    return new MoveCommand(player, player.X + Direction.NE.X, player.Y + Direction.NE.Y);
                case RLKey.Keypad1:
                case RLKey.B:
                    return new MoveCommand(player, player.X + Direction.SW.X, player.Y + Direction.SW.Y);
                case RLKey.Keypad3:
                case RLKey.N:
                    return new MoveCommand(player, player.X + Direction.SE.X, player.Y + Direction.SE.Y);
                case RLKey.Keypad5:
                case RLKey.Period:
                    return new WaitCommand(player);
                #endregion
                case RLKey.Comma:
                    return new PickupCommand(player, Game.Map.Field[player.X, player.Y].ItemStack);
                case RLKey.A:
                    Game.GameMode = Game.Mode.Apply;
                    Game.ShowInventory = true;
                    return null;
                case RLKey.D:
                    Game.GameMode = Game.Mode.Drop;
                    Game.ShowInventory = true;
                    return null;
                case RLKey.E:
                    Game.GameMode = Game.Mode.Equip;
                    Game.ShowInventory = true;
                    return null;
                case RLKey.I:
                    Game.GameMode = Game.Mode.Inventory;
                    Game.ShowInventory = true;
                    return null;
                case RLKey.T:
                    Game.GameMode = Game.Mode.Unequip;
                    Game.ShowEquipment = true;
                    return null;
                case RLKey.Escape:
                    Game.Exit();
                    return null;
                default: return null;
            }
        }

        private static bool HandleMenuInput(RLKeyPress keyPress)
        {
            // TODO: Implement this - Also, please rename it to sth more logical
            switch(keyPress.Key)
            {
                case RLKey.Escape:
                    Game.GameMode = Game.Mode.Normal;
                    Game.ShowInventory = false;
                    Game.ShowEquipment = false;
                    Game.ForceRender();
                    return false;
                default:
                    return true;
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

        private static char ToChar(RLKey key)
        {
            int keyValue = (int)key;
            int charValue;

            switch (keyValue)
            {
                case 51: charValue = ' '; break;
                case 67:
                case 109: charValue = '0'; break;
                case 68:
                case 110: charValue = '1'; break;
                case 69:
                case 111: charValue = '2'; break;
                case 70:
                case 112: charValue = '3'; break;
                case 71:
                case 113: charValue = '4'; break;
                case 72:
                case 114: charValue = '5'; break;
                case 73:
                case 115: charValue = '6'; break;
                case 74:
                case 116: charValue = '7'; break;
                case 75:
                case 117: charValue = '8'; break;
                case 76:
                case 118: charValue = '9'; break;
                case 77:
                case 128: charValue = '/'; break;
                case 78: charValue = '*'; break;
                case 79:
                case 120: charValue = '-'; break;
                case 80:
                case 121: charValue = '+'; break;
                case 81:
                case 127: charValue = '.'; break;
                case 119: charValue = '~'; break;
                case 122: charValue = '['; break;                    
                case 123: charValue = ']'; break;
                case 124: charValue = ';'; break;
                case 125: charValue = '\''; break;
                case 126: charValue = ','; break;
                case 129: charValue = '\\'; break;
                default: charValue = keyValue + 14; break;
            }

            return (char)charValue;
        }
    }
}
