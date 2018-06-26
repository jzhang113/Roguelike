using RLNET;
using Roguelike.Core;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Items;
using System.Text;
using Roguelike.State;

namespace Roguelike.Systems
{
    class StateHandler
    {
        private RLRootConsole _console;
        private Stack<IState> _states;

        private static int _holdTimeout = 0;
        private static bool _holdingKey;
        private const int _HOLD_LIMIT = 15;


        public StateHandler(RLRootConsole console)
        {
            _console = console;
            _states = new Stack<IState>();
            _states.Push(NormalState.Instance);
        }

        public ICommand HandleInput()
        {
            IState currentState = _states.Peek();
            ICommand command = currentState.HandleMouseInput(_console.Mouse);
            if (command != null)
                return command;

            return currentState.HandleKeyInput(_console.Keyboard.GetKeyPress());
        }

        public void PopState()
        {
            _states.Pop();
        }

        public void PushState(IState state)
        {
            _states.Push(state);
        }

        public void Update()
        {
            _states.Peek().Update();
        }

        public void Draw()
        {
            _states.Peek().Draw();
        }

        //public static void temp()
        //{
        //    MapHandler map = Game.Map;
        //    Player player = Game.Player;

        //    var mousePos = GetHoverPosition();
        //    if (mousePos != null)
        //        ResolveMouseInput(mousePos.Value, map, player);

        //    RLKeyPress keyPress = _console.Keyboard.GetKeyPress();

        //    if (keyPress == null)
        //    {
        //        // For some reason, holding a key issues a command, but follows up with nulls.
        //        // We resolve this by making holds somewhat sticky.
        //        //if (_holdingKey)
        //        //{
        //        //    if (_holdTimeout < _HOLD_LIMIT)
        //        //    {
        //        //        _holdTimeout++;
        //        //    }
        //        //    else
        //        //    {
        //        //        Game.ShowOverlay = false;
        //        //        _holdingKey = false;
        //        //        _holdTimeout = 0;
        //        //    }
        //        //}

        //        return null;
        //    }
        //    if (Game.GameState == Mode.Targetting)
        //        return ResolveTargettingInput(keyPress);

        //    if (Game.GameState == Mode.TextInput)
        //        return ResolveTextInput(keyPress);

        //    if (Game.ShowModal)
        //        return ResolveModalInput(keyPress, player);

        //    Game.ShowOverlay = (keyPress.Key == RLKey.Tab);
        //    _holdingKey = true;

        //}

        //#region Mouse Input
        //private static (int X, int Y)? GetHoverPosition()
        //{
        //    RLMouse mouse = _console.Mouse;
        //    int mapTop = Game.Config.MessageView.Height;
        //    int mapBottom = Game.Config.MessageView.Height + Game.Config.MapView.Height;
        //    int mapLeft = 0;
        //    int mapRight = Game.Config.MapView.Width;

        //    if (mouse.X > mapLeft && mouse.X < mapRight - 1 && mouse.Y > mapTop && mouse.Y < mapBottom - 1)
        //    {
        //        int xPos = (mouse.X - mapLeft);
        //        int yPos = (mouse.Y - mapTop);
        //        return (xPos, yPos);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        //private static (int X, int Y)? GetClickPosition()
        //{
        //    return _console.Mouse.GetLeftClick() ? GetHoverPosition() : null;
        //}

        //private static void ResolveMouseInput((int X, int Y) mousePos, MapHandler map, Player player)
        //{
        //    map.ClearHighlight();
        //    Terrain current = map.Field[mousePos.X, mousePos.Y];

        //    if (Game.GameState == Mode.Targetting)
        //    {
        //        IEnumerable<Terrain> path = Game.Map.GetStraightLinePath(player.X, player.Y, mousePos.X, mousePos.Y);
        //        foreach (Terrain tile in path)
        //        {
        //            if (!map.Field[tile.X, tile.Y].IsExplored)
        //            {
        //                break;
        //            }

        //            map.Highlight[tile.X, tile.Y] = RLColor.Red;
        //        }
        //    }
        //    else
        //    {
        //        // TODO: Path may end up broken because an enemy is in the way.
        //        IEnumerable<WeightedPoint> path = map.GetPathToPlayer(mousePos.X, mousePos.Y).Reverse();
        //        bool exploredPathExists = false;

        //        foreach (WeightedPoint p in path)
        //        {
        //            if (!exploredPathExists)
        //                exploredPathExists = true;

        //            if (!map.Field[p.X, p.Y].IsExplored)
        //            {
        //                exploredPathExists = false;
        //                break;
        //            }

        //            map.Highlight[p.X, p.Y] = RLColor.Red;
        //        }

        //        if (current.IsWalkable && exploredPathExists)
        //            map.Highlight[mousePos.X, mousePos.Y] = RLColor.Red;
        //    }

        //    //if (_console.Mouse.GetLeftClick())
        //    //{
        //    //    List<IAction> moves = new List<IAction>();

        //    //    foreach (WeightedPoint p in path)
        //    //        moves.Add(new MoveAction(new TargetZone(TargetShape.Range, (p.X, p.Y))));

        //    //    return new AttackCommand(player, new ActionSequence(100, moves));
        //    //}
        //    if (map.TryGetActor(mousePos.X, mousePos.Y, out Actor displayActor))
        //        LookHandler.DisplayActor(displayActor);

        //    if (map.TryGetItem(mousePos.X, mousePos.Y, out ItemCount displayItem))
        //        LookHandler.DisplayItem(displayItem);

        //    LookHandler.DisplayTerrain(map.Field[mousePos.X, mousePos.Y]);
        //}
        //#endregion

        //#region Target Handling
        //public static void BeginTargetting(ITargetCommand command, Actor source, IAction action)
        //{
        //    _prevMode = Game.GameState;
        //    Game.GameState = Mode.Targetting;
        //    Game.ShowModal = false;
        //    Game.ForceRender();

        //    _targetCommand = command;
        //    _targetSource = source;
        //    _targetAction = action;
        //}

        //private static ICommand CompleteTargetting()
        //{
        //    foreach (Terrain tile in Game.Map.GetTilesInRadius(_targetSource.X, _targetSource.Y, (int)_targetAction.Area.Range))
        //    {
        //        Game.Map.Highlight[tile.X, tile.Y] = Swatch.DbGrass;
        //    }

        //    var clickPos = GetClickPosition();
        //    if (clickPos == null)
        //        return null;

        //    var (clickX, clickY) = clickPos.Value;
        //    int distance = Utils.Distance.EuclideanDistanceSquared(_targetSource.X, _targetSource.Y, clickX, clickY);
        //    float maxRange = _targetAction.Area.Range * _targetAction.Area.Range;

        //    if (distance > maxRange)
        //    {
        //        Game.MessageHandler.AddMessage("Target out of range.");
        //        return null;
        //    }

        //    Game.GameState = _prevMode;
        //    _targetCommand.Target = _targetAction.Area.GetTilesInRange(_targetSource, clickPos.Value);
        //    return _targetCommand;
        //}
        //#endregion

        //#region Text Input Handling
        //public static void BeginTextInput(IInputCommand command)
        //{
        //    _prevMode = Game.GameState;
        //    Game.GameState = Mode.TextInput;
        //    Game.ShowOverlay = true;
        //    OverlayHandler.DisplayText = "Drop how many?";
        //    _inputBuffer.Clear();

        //    _inputCommand = command;
        //}

        //private static ICommand CompleteTextInput(string input)
        //{
        //    Game.GameState = _prevMode;
        //    Game.ShowOverlay = false;

        //    _inputCommand.Input = input;
        //    return _inputCommand;
        //}
        //#endregion

        //#region Input Handling
        //private static ICommand ResolveTargettingInput(RLKeyPress keyPress)
        //{
        //    switch (keyPress.Key)
        //    {
        //        case RLKey.Escape:
        //            Game.GameState = _prevMode;
        //            Game.ForceRender();
        //            break;
        //    }

        //    return CompleteTargetting();
        //}

        //private static ICommand ResolveTextInput(RLKeyPress keyPress)
        //{
        //    switch (keyPress.Key)
        //    {
        //        case RLKey.Escape:
        //            Game.GameState = _prevMode;
        //            Game.ShowOverlay = false;
        //            Game.ForceRender();
        //            break;
        //        case RLKey.BackSpace:
        //            if (_inputBuffer.Length > 0) _inputBuffer.Length--;
        //            break;
        //        case RLKey.Enter:
        //        case RLKey.KeypadEnter:
        //            return CompleteTextInput(_inputBuffer.ToString());
        //        default:
        //            _inputBuffer.Append(ToChar(keyPress.Key));
        //            break;
        //    }

        //    OverlayHandler.DisplayText = $"Drop how many? {_inputBuffer}";
        //    return null;
        //}

        //private static ICommand ResolveModalInput(RLKeyPress keyPress, Actor player)
        //{
        //    switch (keyPress.Key)
        //    {
        //        case RLKey.Escape:
        //            Game.GameState = Mode.Normal;
        //            Game.ShowModal = false;
        //            Game.ShowEquipment = false;
        //            Game.ForceRender();
        //            return null;
        //    }

        //    char keyChar = ToChar(keyPress.Key);
        //    switch (Game.GameState)
        //    {
        //        case Mode.Inventory:
        //            bool opened = Game.Player.Inventory.OpenStack(keyChar);
        //            if (opened)
        //            {
        //                Game.GameState = Mode.SubInv;
        //            }

        //            if (Game.Player.Inventory.HasKey(keyChar))
        //            {
        //                Game.GameState = Mode.InvMenu;
        //            }
        //            // TODO: implement inventory actions
        //            return null;
        //        case Mode.Apply:
        //            return new ApplyCommand(player, keyChar);
        //        case Mode.Drop:
        //            return new DropCommand(player, keyChar);
        //        case Mode.Equip:
        //            return new EquipCommand(player, keyChar);
        //        case Mode.Unequip:
        //            return new UnequipCommand(player, keyChar);
        //        default:
        //            return null;
        //    }
        //}

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
