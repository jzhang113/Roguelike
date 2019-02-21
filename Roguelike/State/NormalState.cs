using BearLib;
using Optional;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Input;
using Roguelike.Items;
using Roguelike.Systems;
using Roguelike.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.State
{
    internal sealed class NormalState : IState
    {
        private static readonly Lazy<NormalState> _instance = new Lazy<NormalState>(() => new NormalState());
        public static NormalState Instance => _instance.Value;

        private NormalState()
        {
        }

        // ReSharper disable once CyclomaticComplexity
        public Option<ICommand> HandleKeyInput(int key)
        {
            Player player = Game.Player;
            Weapon weapon = player.Equipment.PrimaryWeapon;

            switch (InputMapping.GetNormalInput(key))
            {
                //case NormalInput.None:
                //    return null;

                #region Movement Keys
                case NormalInput.MoveW:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X + Direction.W.X, player.Y));
                case NormalInput.MoveS:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X, player.Y + Direction.S.Y));
                case NormalInput.MoveN:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X, player.Y + Direction.N.Y));
                case NormalInput.MoveE:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X + Direction.E.X, player.Y));
                case NormalInput.MoveNW:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X + Direction.NW.X, player.Y + Direction.NW.Y));
                case NormalInput.MoveNE:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X + Direction.NE.X, player.Y + Direction.NE.Y));
                case NormalInput.MoveSW:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X + Direction.SW.X, player.Y + Direction.SW.Y));
                case NormalInput.MoveSE:
                    return Option.Some<ICommand>(new MoveCommand(player, player.X + Direction.SE.X, player.Y + Direction.SE.Y));
                case NormalInput.Wait:
                    return Option.Some<ICommand>(new WaitCommand(player));
                #endregion

                case NormalInput.Get:
                    return Option.Some<ICommand>(new PickupCommand(player, Game.Map.GetStack(player.X, player.Y)));
                case NormalInput.Throw:
                    // TODO: Add ability to throw without wielding
                    if (weapon == null)
                    {
                        Game.MessageHandler.AddMessage("Nothing to throw.");
                        return Option.None<ICommand>();
                    }

                    IAction thrown = weapon.Throw();
                    Game.StateHandler.PushState(new TargettingState(
                        player,
                        thrown.Area,
                        returnTarget =>
                        {
                            Game.MessageHandler.AddMessage($"You throw a {weapon}.");

                            // Switch to offhand weapon if possible.
                            player.Equipment.PrimaryWeapon = player.Equipment.OffhandWeapon;

                            // Drop the item on the map.
                            // TODO: Add possibility of weapon stuck / breaking
                            // TODO: Handle case of multiple thrown at once?
                            Loc[] enumerable = returnTarget as Loc[] ?? returnTarget.ToArray();
                            Loc point = enumerable[0];
                            weapon.X = point.X;
                            weapon.Y = point.Y;
                            Game.Map.AddItem(weapon);

                            return new DelayActionCommand(player, thrown, enumerable);
                        }));
                    return Option.None<ICommand>();
                case NormalInput.ChangeLevel:
                    return Option.Some<ICommand>(new ChangeLevelCommand(Game.Map.TryChangeLocation(player)));
                case NormalInput.OpenApply:
                    Game.StateHandler.PushState(ApplyState.Instance);
                    return Option.None<ICommand>();
                case NormalInput.OpenDrop:
                    Game.StateHandler.PushState(DropState.Instance);
                    return Option.None<ICommand>();
                case NormalInput.OpenEquip:
                    Game.StateHandler.PushState(EquipState.Instance);
                    return Option.None<ICommand>();
                case NormalInput.OpenInventory:
                    Game.StateHandler.PushState(InventoryState.Instance);
                    return Option.None<ICommand>();
                case NormalInput.OpenUnequip:
                    Game.StateHandler.PushState(UnequipState.Instance);
                    return Option.None<ICommand>();
                case NormalInput.AutoExplore:
                    Game.StateHandler.PushState(AutoexploreState.Instance);
                    return Option.None<ICommand>();
                case NormalInput.OpenMenu:
                    Game.Exit();
                    return Option.None<ICommand>();
            }

            // HACK: debugging commands
            if (key == Terminal.TK_Q)
            {
                // NOTE: Movement occurs after the hook is used. This means that using the hook
                // to pull enemies will often give the Player a first hit on enemies, but using
                // the hook to escape will give enemies an "attack of opportunity".
                IAction hookAction = new HookAction(10);
                Game.StateHandler.PushState(new TargettingState(
                    player,
                    hookAction.Area,
                    returnTarget => new DelayActionCommand(player, hookAction, returnTarget)));
                return Option.None<ICommand>();
            }

            // TODO: Create a debug menu for test commands
            if (key == Terminal.TK_GRAVE)
            {
                Game.EventScheduler.Clear();
                Game.NewGame();
                return Option.Some<ICommand>(new WaitCommand(player));
            }

            // TODO: Use weapon's attack sequence if equipped???
            IAction action = player.GetBasicAttack();
            if (key == Terminal.TK_Z)
            {
                action = weapon?.AttackLeft() ?? player.GetBasicAttack();
            }
            else if (key == Terminal.TK_X)
            {
                action = weapon?.AttackRight() ?? player.GetBasicAttack();
            }
            else if (Terminal.Check(Terminal.TK_SHIFT))
            {
                // TODO: Change to an arbitrary facing
                // TODO: Should attacks update facing?
                player.Facing = player.Facing.Right();
                Game.Map.Refresh();
                return Option.None<ICommand>();
            }

            if (Terminal.Check(Terminal.TK_SHIFT))
            {
                Game.StateHandler.PushState(new TargettingState(
                    player,
                    action.Area,
                    target => new DelayActionCommand(player, action, target)));
                return Option.None<ICommand>();
            }
            else
            {
                (int dx, int dy) = player.Facing;
                IEnumerable<Loc> target = action.Area.GetTilesInRange(
                    player,
                    new Loc(player.X + dx, player.Y + dy));
                return Option.Some<ICommand>(new DelayActionCommand(player, action, target));
            }
        }

        public Option<ICommand> HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            Tile current = Game.Map.Field[x, y];
            if (!current.IsExplored || current.IsWall)
                return Option.None<ICommand>();

            IEnumerable<WeightedPoint> path = Game.Map.GetPathToPlayer(x, y).Reverse();
            foreach (WeightedPoint p in path)
            {
                if (Game.Map.Field[p.X, p.Y].IsExplored)
                    Game.Overlay.Set(p.X, p.Y, Colors.Path);
            }

            Game.Overlay.Set(current.X, current.Y, Colors.Cursor);

            if (Game.Map.Field[current.X, current.Y].IsVisible)
            {
                Game.Map.GetItem(current.X, current.Y).MatchSome(LookPanel.DisplayItem);
                Game.Map.GetActor(current.X, current.Y).MatchSome(LookPanel.DisplayActor);
            }
            else
            {
                LookPanel.Clear();
            }

            LookPanel.DisplayTerrain(Game.Map.Field[current.X, current.Y]);

            return Option.None<ICommand>();
        }

        public void Update(ICommand command)
        {
            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
        }

        public void Draw(LayerInfo layer)
        {
            Game.Threatened.Draw(layer);
            Game.Map.Draw(layer);
        }
    }
}
