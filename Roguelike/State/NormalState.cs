using RLNET;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Input;
using Roguelike.Items;
using Roguelike.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

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
                ability = player.Equipment.PrimaryWeapon?.GetAbility(0);
            else if (keyPress.Alt)
                ability = player.Equipment.PrimaryWeapon?.GetAbility(1);
            else if (keyPress.Control)
                ability = player.Equipment.PrimaryWeapon?.GetAbility(2);

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
                //case NormalInput.None:
                //    return null;

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
                        return new ChangeLevelCommand(destination);
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
                Game.EventScheduler.Clear();
                Game.NewGame();
                return new WaitCommand(Game.Player);
            }

            return null;
        }

        // ReSharper disable once CyclomaticComplexity
        private static ICommand ResolveAttackInput(RLKeyPress keyPress, Actor player, IAction ability)
        {
            // TODO: merge with other resolve block?
            NormalInput input = InputMapping.GetNormalInput(keyPress);
            switch (input)
            {
                case NormalInput.AttackW:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X + Direction.W.X, player.Y]);
                case NormalInput.AttackN:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X, player.Y + Direction.S.Y]);
                case NormalInput.AttackE:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X, player.Y + Direction.N.Y]);
                case NormalInput.AttackS:
                    return new ActionCommand(player, ability, Game.Map.Field[player.X + Direction.E.X, player.Y]);
                case NormalInput.AttackNW:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.NW.X, player.Y + Direction.NW.Y]);
                case NormalInput.AttackNE:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.NE.X, player.Y + Direction.NE.Y]);
                case NormalInput.AttackSW:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.SW.X, player.Y + Direction.SW.Y]);
                case NormalInput.AttackSE:
                    return new ActionCommand(player, ability,
                        Game.Map.Field[player.X + Direction.SE.X, player.Y + Direction.SE.Y]);
                default: return null;
            }
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            if (!MouseInput.GetHoverPosition(mouse, out (int X, int Y) mousePos))
                return null;

            Tile current = Game.Map.Field[mousePos.X, mousePos.Y];
            if (!current.IsExplored || current.IsWall)
                return null;

            IEnumerable<WeightedPoint> path = Game.Map.GetPathToPlayer(mousePos.X, mousePos.Y).Reverse();
            foreach (WeightedPoint p in path)
            {
                if (Game.Map.Field[p.X, p.Y].IsExplored)
                    Game.OverlayHandler.Set(p.X, p.Y, Colors.PathColor);
            }

            Game.OverlayHandler.Set(current.X, current.Y, Colors.Cursor);

            if (Game.Map.Field[current.X, current.Y].IsVisible
                && Game.Map.TryGetActor(current.X, current.Y, out Actor displayActor))
                LookHandler.DisplayActor(displayActor);
            else if (Game.Map.TryGetItem(current.X, current.Y, out ItemCount displayItem)
                && displayItem.Count > 0)
                LookHandler.DisplayItem(displayItem);
            else
                LookHandler.Clear();

            LookHandler.DisplayTerrain(Game.Map.Field[current.X, current.Y]);

            return null;
        }

        public void Update()
        {
            Game.OverlayHandler.ClearForeground();
            Game.Player.NextCommand = Game.StateHandler.HandleInput();
            Game.EventScheduler.Run();
            Game.ForceRender();
        }

        public void Draw()
        {
            Game.Map.Draw(Game.MapConsole);
            Game.OverlayHandler.Draw(Game.MapConsole);
        }
    }
}
