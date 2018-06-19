using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Items;

namespace Roguelike.Systems
{
    static class InputHandler
    {
        private static RLRootConsole _console;
        private static int _holdTimeout;
        private static bool _holdingKey;
        private const int _HOLD_LIMIT = 15;

        private static ITargettable _targettingCommand;
        private static Actor _targettingSource;
        private static IAction _targettingAction;

        static InputHandler()
        {
            _holdTimeout = 0;
        }

        public static void Initialize(RLRootConsole console)
        {
            _console = console;
        }

        public static ICommand HandleInput()
        {
            MapHandler map = Game.Map;
            Player player = Game.Player;

            var mousePos = GetHoverPosition();
            if (mousePos != null)
                ResolveMouseInput(mousePos.Value, map, player);

            RLKeyPress keyPress = _console.Keyboard.GetKeyPress();

            if (keyPress == null)
            {
                // For some reason, holding a key issues a command, but follows up with nulls.
                // We resolve this by making holds somewhat sticky.
                if (_holdingKey)
                {
                    if (_holdTimeout < _HOLD_LIMIT)
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
            if (Game.GameMode == Enums.Mode.Targetting)
                return ResolveTargettingInput(keyPress);

            if (Game.ShowModal)
                return ResolveModalInput(keyPress, player);

            Game.ShowOverlay = (keyPress.Key == RLKey.Tab);
            _holdingKey = true;

            IAction ability = null;
            if (keyPress.Shift)
                ability = player.Equipment.PrimaryWeapon.GetAbility(0);
            else if (keyPress.Alt)
                ability = player.Equipment.PrimaryWeapon.GetAbility(1);
            else if (keyPress.Control)
                ability = player.Equipment.PrimaryWeapon.GetAbility(2);

            return ability != null
                ? ResolveAttackInput(keyPress, player, ability)
                : ResolveInput(keyPress, player);
        }

        #region Mouse Input
        private static (int X, int Y)? GetHoverPosition()
        {
            RLMouse mouse = _console.Mouse;
            int mapTop = Game.Config.MessageView.Height;
            int mapBottom = Game.Config.MessageView.Height + Game.Config.MapView.Height;
            int mapLeft = 0;
            int mapRight = Game.Config.MapView.Width;

            if (mouse.X > mapLeft && mouse.X < mapRight - 1 && mouse.Y > mapTop && mouse.Y < mapBottom - 1)
            {
                int xPos = (mouse.X - mapLeft);
                int yPos = (mouse.Y - mapTop);
                return (xPos, yPos);
            }
            else
            {
                return null;
            }
        }

        private static (int X, int Y)? GetClickPosition()
        {
            return _console.Mouse.GetLeftClick() ? GetHoverPosition() : null;
        }

        private static void ResolveMouseInput((int X, int Y) mousePos, MapHandler map, Player player)
        {
            map.ClearHighlight();
            Terrain current = map.Field[mousePos.X, mousePos.Y];

            if (Game.GameMode == Enums.Mode.Targetting)
            {
                IEnumerable<Terrain> path = Game.Map.GetStraightLinePath(player.X, player.Y, mousePos.X, mousePos.Y);
                foreach (Terrain tile in path)
                {
                    if (!map.Field[tile.X, tile.Y].IsExplored)
                    {
                        break;
                    }

                    map.Highlight[tile.X, tile.Y] = RLColor.Red;
                }
            }
            else
            {
                // TODO: Path may end up broken because an enemy is in the way.
                IEnumerable<WeightedPoint> path = map.GetPathToPlayer(mousePos.X, mousePos.Y).Reverse();
                bool exploredPathExists = false;

                foreach (WeightedPoint p in path)
                {
                    if (!exploredPathExists)
                        exploredPathExists = true;

                    if (!map.Field[p.X, p.Y].IsExplored)
                    {
                        exploredPathExists = false;
                        break;
                    }

                    map.Highlight[p.X, p.Y] = RLColor.Red;
                }

                if (current.IsWalkable && exploredPathExists)
                    map.Highlight[mousePos.X, mousePos.Y] = RLColor.Red;
            }

            //if (_console.Mouse.GetLeftClick())
            //{
            //    List<IAction> moves = new List<IAction>();

            //    foreach (WeightedPoint p in path)
            //        moves.Add(new MoveAction(new TargetZone(TargetShape.Range, (p.X, p.Y))));

            //    return new AttackCommand(player, new ActionSequence(100, moves));
            //}
            if (map.TryGetActor(mousePos.X, mousePos.Y, out Actor displayActor))
                LookHandler.DisplayActor(displayActor);

            if (map.TryGetItem(mousePos.X, mousePos.Y, out ItemInfo displayItem))
                LookHandler.DisplayItem(displayItem);

            LookHandler.DisplayTerrain(map.Field[mousePos.X, mousePos.Y]);
        }
        #endregion

        #region Target Handling
        public static void BeginTargetting(ITargettable command, Actor source, IAction action)
        {
            Game.GameMode = Enums.Mode.Targetting;
            Game.ShowModal = false;
            Game.ForceRender();

            _targettingCommand = command;
            _targettingSource = source;
            _targettingAction = action;

            ResolveTargetting();
        }

        private static ICommand ResolveTargetting()
        {
            foreach (Terrain tile in Game.Map.GetTilesInRadius(_targettingSource.X, _targettingSource.Y, (int)_targettingAction.Area.Range))
            {
                Game.Map.Highlight[tile.X, tile.Y] = Swatch.DbGrass;
            }

            var clickPos = GetClickPosition();
            if (clickPos != null)
            {
                var (clickX, clickY) = clickPos.Value;
                int distance = Utils.Distance.EuclideanDistanceSquared(_targettingSource.X, _targettingSource.Y, clickX, clickY);
                float maxRange = _targettingAction.Area.Range * _targettingAction.Area.Range;

                if (distance <= maxRange)
                {
                    Game.GameMode = Enums.Mode.Normal;
                    _targettingCommand.Target = _targettingAction.Area.GetTilesInRange(_targettingSource, clickPos.Value);
                    return _targettingCommand as ICommand;
                }
                else
                {
                    Game.MessageHandler.AddMessage("Target out of range.");
                }
            }

            return null;
        }
        #endregion

        #region Input Handling
        private static ICommand ResolveTargettingInput(RLKeyPress keyPress)
        {
            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    Game.GameMode = Enums.Mode.Normal;
                    Game.ForceRender();
                    break;
            }

            return ResolveTargetting();
        }

        private static ICommand ResolveModalInput(RLKeyPress keyPress, Actor player)
        {
            switch (keyPress.Key)
            {
                case RLKey.Escape:
                    Game.GameMode = Enums.Mode.Normal;
                    Game.ShowModal = false;
                    Game.ShowEquipment = false;
                    Game.ForceRender();
                    return null;
            }

            char keyChar = ToChar(keyPress.Key);
            switch (Game.GameMode)
            {
                case Enums.Mode.Inventory:
                    // TODO: implement inventory actions
                    return null;
                case Enums.Mode.Apply:
                    return new ApplyCommand(player, keyChar);
                case Enums.Mode.Drop:
                    return new DropCommand(player, keyChar);
                case Enums.Mode.Equip:
                    return new EquipCommand(player, keyChar);
                case Enums.Mode.Unequip:
                    return new UnequipCommand(player, keyChar);
                default:
                    return null;
            }
        }

        // ReSharper disable once CyclomaticComplexity
        private static ICommand ResolveAttackInput(RLKeyPress keyPress, Player player, IAction ability)
        {
            switch (keyPress.Key)
            {
                case RLKey.Left:
                case RLKey.Keypad4:
                case RLKey.H:
                    return new AttackCommand(player, ability, Game.Map.Field[player.X + Direction.W.X, player.Y]);
                case RLKey.Down:
                case RLKey.Keypad2:
                case RLKey.J:
                    return new AttackCommand(player, ability, Game.Map.Field[player.X, player.Y + Direction.S.Y]);
                case RLKey.Up:
                case RLKey.Keypad8:
                case RLKey.K:
                    return new AttackCommand(player, ability, Game.Map.Field[player.X, player.Y + Direction.N.Y]);
                case RLKey.Right:
                case RLKey.Keypad6:
                case RLKey.L:
                    return new AttackCommand(player, ability, Game.Map.Field[player.X + Direction.E.X, player.Y]);
                case RLKey.Keypad7:
                case RLKey.Y:
                    return new AttackCommand(player, ability,
                        Game.Map.Field[player.X + Direction.NW.X, player.Y + Direction.NW.Y]);
                case RLKey.Keypad9:
                case RLKey.U:
                    return new AttackCommand(player, ability,
                        Game.Map.Field[player.X + Direction.NE.X, player.Y + Direction.NE.Y]);
                case RLKey.Keypad1:
                case RLKey.B:
                    return new AttackCommand(player, ability,
                        Game.Map.Field[player.X + Direction.SW.X, player.Y + Direction.SW.Y]);
                case RLKey.Keypad3:
                case RLKey.N:
                    return new AttackCommand(player, ability,
                        Game.Map.Field[player.X + Direction.SE.X, player.Y + Direction.SE.Y]);
                default: return null;
            }
        }

        // ReSharper disable once CyclomaticComplexity
        private static ICommand ResolveInput(RLKeyPress keyPress, Actor player)
        {
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
                    // TODO: only grabs top item
                    if (Game.Map.TryGetStack(player.X, player.Y, out InventoryHandler stack))
                        return new PickupCommand(player, stack);
                    else
                        return null;
                case RLKey.BackSlash:
                    // HACK: Ad-hoc input handling
                    if (Game.Map.TryChangeLocation(player, out string destination))
                        return new ChangeLevelCommand(player, destination);
                    else
                        return null;
                case RLKey.A:
                    Game.GameMode = Enums.Mode.Apply;
                    Game.ShowModal = true;
                    return null;
                case RLKey.D:
                    Game.GameMode = Enums.Mode.Drop;
                    Game.ShowModal = true;
                    return null;
                case RLKey.E:
                    Game.GameMode = Enums.Mode.Equip;
                    Game.ShowModal = true;
                    return null;
                case RLKey.I:
                    Game.GameMode = Enums.Mode.Inventory;
                    Game.ShowModal = true;
                    return null;
                case RLKey.T:
                    Game.GameMode = Enums.Mode.Unequip;
                    Game.ShowEquipment = true;
                    return null;
                case RLKey.R:
                    Game.NewGame();
                    return null;
                case RLKey.Escape:
                    Game.Exit();
                    return null;
                default: return null;
            }
        }
        #endregion

        // ReSharper disable once CyclomaticComplexity
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
