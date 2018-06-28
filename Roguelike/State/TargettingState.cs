using RLNET;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Systems;
using System;
using System.Collections.Generic;

namespace Roguelike.State
{
    class TargettingState: IState
    {
        private readonly Actor _targetSource;
        private readonly IAction _targetAction;

        public TargettingState(Actor source, IAction action)
        {
            _targetSource = source;
            _targetAction = action;

            OverlayHandler.DisplayText = "targetting mode";
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            // TODO
            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            foreach (Terrain tile in Game.Map.GetTilesInRadius(_targetSource.X, _targetSource.Y, (int)_targetAction.Area.Range))
            {
                Game.Map.Highlight[tile.X, tile.Y] = Swatch.DbGrass;
            }

            if (!MouseHandler.GetClickPosition(mouse, out (int X, int Y) click))
                return null;

            int distance = Utils.Distance.EuclideanDistanceSquared(_targetSource.X, _targetSource.Y, click.X, click.Y);
            double maxRange = _targetAction.Area.Range * _targetAction.Area.Range;

            if (distance > maxRange)
            {
                Game.MessageHandler.AddMessage("Target out of range.");
                return null;
            }

            IEnumerable<Terrain> targets = _targetAction.Area.GetTilesInRange(_targetSource, click);
            return OnSubmit(new TargettingEventArgs(targets));
        }

        public void Update()
        {
            Game.ForceRender();
            ICommand command = Game.StateHandler.HandleInput();
            if (command == null)
                return;

            if (EventScheduler.Execute(Game.Player, command))
                Game.StateHandler.PopState();
        }

        public void Draw()
        {
            OverlayHandler.Draw(Game.MapConsole);
            RLConsole.Blit(Game.MapConsole, 0, 0, Game.Config.MapView.Width, Game.Config.MapView.Height, Game.RootConsole, 0, Game.Config.MessageView.Height);

            //IEnumerable<Terrain> path = Game.Map.GetStraightLinePath(Game.Player.X, Game.Player.Y, mousePos.X, mousePos.Y);
            //foreach (Terrain tile in path)
            //{
            //    if (!Game.Map.Field[tile.X, tile.Y].IsExplored)
            //    {
            //        break;
            //    }

            //    Game.Map.Highlight[tile.X, tile.Y] = RLColor.Red;
            //}
        }

        public event CommandEventHandler<TargettingEventArgs> Submit;

        protected virtual ICommand OnSubmit(TargettingEventArgs e)
        {
            return Submit?.Invoke(this, e);
        }

        public class TargettingEventArgs : EventArgs
        {
            public IEnumerable<Terrain> Target { get; }

            public TargettingEventArgs(IEnumerable<Terrain> target)
            {
                Target = target;
            }
        }
    }
}
