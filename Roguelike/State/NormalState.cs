using RLNET;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Input;
using Roguelike.Systems;
using System;

namespace Roguelike.State
{
    class NormalState : IState
    {
        private static readonly Lazy<NormalState> _instance = new Lazy<NormalState>(() => new NormalState());
        public static NormalState Instance => _instance.Value;

        private NormalState()
        {
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            if (keyPress == null)
                return null;

            Player player = Game.Player;
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

        // ReSharper disable once CyclomaticComplexity
        private static ICommand ResolveInput(RLKeyPress keyPress, Actor player)
        {
            NormalInput input = InputMapping.GetNormalInput(keyPress);
            switch (input)
            {
                case NormalInput.None:
                    return null;

                #region Movement Keys
                case NormalInput.MoveW:
                    return new MoveCommand(player, player.X + Direction.W.X, player.Y);
                case NormalInput.MoveS:
                    return new MoveCommand(player, player.X, player.Y + Direction.S.Y);
                case NormalInput.MoveN:
                    return new MoveCommand(player, player.X, player.Y + Direction.N.Y);
                case NormalInput.MoveE:
                    return new MoveCommand(player, player.X + Direction.E.X, player.Y);
                case NormalInput.MoveNW:
                    return new MoveCommand(player, player.X + Direction.NW.X, player.Y + Direction.NW.Y);
                case NormalInput.MoveNE:
                    return new MoveCommand(player, player.X + Direction.NE.X, player.Y + Direction.NE.Y);
                case NormalInput.MoveSW:
                    return new MoveCommand(player, player.X + Direction.SW.X, player.Y + Direction.SW.Y);
                case NormalInput.MoveSE:
                    return new MoveCommand(player, player.X + Direction.SE.X, player.Y + Direction.SE.Y);
                case NormalInput.Wait:
                    return new WaitCommand(player);
                #endregion

                case NormalInput.Get:
                    // TODO: only grabs top item
                    if (Game.Map.TryGetStack(player.X, player.Y, out InventoryHandler stack))
                        return new PickupCommand(player, stack);
                    else
                        Game.MessageHandler.AddMessage("Nothing to pick up here.");
                    return null;
                case NormalInput.ChangeLevel:
                    // HACK: Ad-hoc input handling
                    if (Game.Map.TryChangeLocation(player, out World.LevelId destination))
                        return new ChangeLevelCommand(player, destination);
                    else
                        Game.MessageHandler.AddMessage("There are no exits here.");
                    return null;
                case NormalInput.OpenApply:
                    Game.StateHandler.PushState(ApplyState.Instance);
                    return null;
                case NormalInput.OpenDrop:
                    Game.StateHandler.PushState(DropState.Instance);
                    return null;
                case NormalInput.OpenEquip:
                    Game.StateHandler.PushState(EquipState.Instance);
                    return null;
                case NormalInput.OpenInventory:
                    Game.StateHandler.PushState(InventoryState.Instance);
                    return null;
                case NormalInput.OpenUnequip:
                    Game.StateHandler.PushState(UnequipState.Instance);
                    return null;
                case NormalInput.AutoExplore:
                    Game.StateHandler.PushState(AutoexploreState.Instance);
                    return null;
                case NormalInput.OpenMenu:
                    Game.Exit();
                    return null;
            }

            // HACK: debugging commands
            if (keyPress.Key == RLKey.Q)
            {
                // NOTE: Movement occurs after the hook is used. This means that using the hook
                // to pull enemies will often give the Player a first hit on enemies, but using
                // the hook to escape will give enemies an "attack of opportunity".
                IAction hookAction = new HookAction(10);
                Game.StateHandler.PushState(new TargettingState(
                    Game.Player,
                    hookAction.Area,
                    returnTarget => new ActionCommand(Game.Player, hookAction, returnTarget)));
                return null;
            }

            if (keyPress.Key == RLKey.W)
            {
                Game.StateHandler.PushState(new AnimationState(new Animations.SpinAnimation(Game.Player.X, Game.Player.Y)));
                return null;
            }

            if (keyPress.Key == RLKey.R)
            {
                Game.NewGame();
                return null;
            }

            return null;
        }

        // ReSharper disable once CyclomaticComplexity
        private static ICommand ResolveAttackInput(RLKeyPress keyPress, Actor player, IAction ability)
        {
            switch (keyPress.Key)
            {
                case RLKey.Left:
                case RLKey.Keypad4:
                case RLKey.H:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X + Direction.W.X, player.Y]);
                case RLKey.Down:
                case RLKey.Keypad2:
                case RLKey.J:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X, player.Y + Direction.S.Y]);
                case RLKey.Up:
                case RLKey.Keypad8:
                case RLKey.K:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X, player.Y + Direction.N.Y]);
                case RLKey.Right:
                case RLKey.Keypad6:
                case RLKey.L:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X + Direction.E.X, player.Y]);
                case RLKey.Keypad7:
                case RLKey.Y:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.NW.X, player.Y + Direction.NW.Y]);
                case RLKey.Keypad9:
                case RLKey.U:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.NE.X, player.Y + Direction.NE.Y]);
                case RLKey.Keypad1:
                case RLKey.B:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.SW.X, player.Y + Direction.SW.Y]);
                case RLKey.Keypad3:
                case RLKey.N:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.SE.X, player.Y + Direction.SE.Y]);
                default: return null;
            }
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            //    map.ClearHighlight();
            //    Terrain current = map.Field[mousePos.X, mousePos.Y];

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
            //    

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
            return null;
        }

        public void Update()
        {
            Game.Player.NextCommand = Game.StateHandler.HandleInput();
            Game.EventScheduler.Run();
            Game.ForceRender();
        }

        public void Draw()
        {
            Game.MapConsole.Clear(0, RLColor.Black, Colors.TextHeading, 0);
            Game.Map.Draw(Game.MapConsole);
        }
    }
}
