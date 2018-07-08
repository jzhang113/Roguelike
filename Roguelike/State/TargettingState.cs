using RLNET;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Systems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.State
{
    class TargettingState : IState
    {
        private readonly Actor _targetSource;
        private readonly IAction _targetAction;
        private readonly Func<IEnumerable<Terrain>, ICommand> _callback;
        private readonly IEnumerable<Terrain> _inRange;

        public TargettingState(Actor source, IAction action, Func<IEnumerable<Terrain>, ICommand> callback)
        {
            _targetSource = source;
            _targetAction = action;
            _callback = callback;

            Game.OverlayHandler.DisplayText = "targetting mode";
            Game.OverlayHandler.ClearBackground();
            _inRange = Game.Map.GetTilesInRadius(_targetSource.X, _targetSource.Y, (int)_targetAction.Area.Range).ToList();
            foreach (Terrain tile in _inRange)
            {
                if (tile.IsVisible)
                    Game.OverlayHandler.Set(tile.X, tile.Y, Swatch.DbGrass, true);
            }
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            // TODO
            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            if (!MouseHandler.GetHoverPosition(mouse, out (int X, int Y) hover))
                return null;

            Game.OverlayHandler.ClearForeground();
            (int X, int Y) click = (_targetSource.X, _targetSource.Y);

            foreach (Terrain highlight in Game.Map.GetStraightLinePath(_targetSource.X, _targetSource.Y,
                hover.X, hover.Y))
            {
                if (!_inRange.Contains(highlight))
                    break;

                Game.OverlayHandler.Set(highlight.X, highlight.Y, Swatch.DbSun);
                click = (highlight.X, highlight.Y);

                if (!highlight.IsWalkable)
                    break;
            }

            Game.OverlayHandler.Set(click.X, click.Y, Swatch.DbBlood);

            if (!mouse.GetLeftClick())
                return null;

            IEnumerable<Terrain> targets = _targetAction.Area.GetTilesInRange(_targetSource, click);
            return _callback(targets);
        }

        public void Update()
        {
            Game.ForceRender();
            ICommand command = Game.StateHandler.HandleInput();
            if (command == null)
                return;

            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
            Game.StateHandler.PopState();

            if (command.Animation != null)
                Game.StateHandler.PushState(new AnimationState(command.Animation));
        }

        public void Draw()
        {
            Game.OverlayHandler.Draw(Game.MapConsole);
        }
    }
}
