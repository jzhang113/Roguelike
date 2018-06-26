using System;
using RLNET;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Systems;

namespace Roguelike.State
{
    class NormalState : IState
    {
        private static Lazy<NormalState> _instance = new Lazy<NormalState>(() => new NormalState());
        public static NormalState Instance => _instance.Value;

        private bool _render;

        private NormalState()
        {
            _render = true;
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

        private ICommand ResolveInput(RLKeyPress keyPress, Player player)
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
                    Game.StateHandler.PushState(ApplyState.Instance);
                    return null;
                case RLKey.D:
                    Game.StateHandler.PushState(DropState.Instance);
                    return null;
                case RLKey.E:
                    Game.StateHandler.PushState(EquipState.Instance);
                    return null;
                case RLKey.I:
                    Game.StateHandler.PushState(InventoryState.Instance);
                    return null;
                case RLKey.T:
                    Game.StateHandler.PushState(UnequipState.Instance);
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

        private ICommand ResolveAttackInput(RLKeyPress keyPress, Player player, IAction ability)
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

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            // TODO
            return null;
        }

        public void Cleanup()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            while (Game.EventScheduler.Update())
            {
                _render = true;
            }
        }

        public void Draw()
        {
            if (Game.MessageHandler.Redraw || _render)
            {
                Game._messageConsole.Clear(0, Swatch.DbDeepWater, Colors.TextHeading);
                Game.MessageHandler.Draw(Game._messageConsole);
                RLConsole.Blit(Game._messageConsole, 0, 0, Game.Config.MessageView.Width, Game.Config.MessageView.Height, Game._rootConsole, 0, 0);
            }

            if (_render)
            {
                Game.Map.ClearHighlight();

                Game._statConsole.Clear(0, Swatch.DbOldStone, Colors.TextHeading);
                RLConsole.Blit(Game._statConsole, 0, 0, Game.Config.StatView.Width, Game.Config.StatView.Height, Game._rootConsole, 0, Game.Config.MessageView.Height + Game.Config.MapView.Height);

                _render = false;
            }

            Game._mapConsole.Clear(0, RLColor.Black, Colors.TextHeading, 0);
            Game.Map.Draw(Game._mapConsole);

            //if (GameState == Enums.Mode.Targetting)
            //    _mapConsole.Print(1, 1, "targetting mode", Colors.TextHeading);

            Game._viewConsole.Clear(0, Swatch.DbWood, Colors.TextHeading);
            LookHandler.Draw(Game._viewConsole);

            RLConsole.Blit(Game._mapConsole, 0, 0, Game.Config.MapView.Width, Game.Config.MapView.Height, Game._rootConsole, 0, Game.Config.MessageView.Height);
            RLConsole.Blit(Game._viewConsole, 0, 0, Game.Config.ViewWindow.Width, Game.Config.ViewWindow.Height, Game._rootConsole, Game.Config.Map.Width, 0);
        }
    }
}
