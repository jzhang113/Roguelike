using RLNET;
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
        private readonly Actor _source;
        private readonly TargetZone _targetZone;
        private readonly Func<IEnumerable<Terrain>, ICommand> _callback;
        private readonly IEnumerable<Terrain> _inRange;

        public TargettingState(Actor source, TargetZone zone, Func<IEnumerable<Terrain>, ICommand> callback)
        {
            _source = source;
            _targetZone = zone;
            _callback = callback;

            Game.OverlayHandler.DisplayText = "targetting mode";
            Game.OverlayHandler.ClearBackground();
            _inRange = Game.Map.GetTilesInRadius(_source.X, _source.Y, (int)_targetZone.Range).ToList();
            foreach (Terrain tile in _inRange)
            {
                foreach (Terrain pathTile in Game.Map.GetStraightLinePath(_source.X, _source.Y, tile.X, tile.Y))
                {
                    Game.OverlayHandler.Set(pathTile.X, pathTile.Y, Swatch.DbGrass, true);

                    if (!pathTile.IsWalkable)
                        break;
                }
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
            (int X, int Y) click = (_source.X, _source.Y);

            foreach (Terrain highlight in Game.Map.GetStraightLinePath(_source.X, _source.Y,
                hover.X, hover.Y))
            {
                if (!_inRange.Contains(highlight))
                    break;

                Game.OverlayHandler.Set(highlight.X, highlight.Y, Swatch.DbSun);
                click = (highlight.X, highlight.Y);

                if (!highlight.IsWalkable)
                    break;
            }


            IEnumerable<Terrain> targets = _targetZone.GetTilesInRange(_source, click).ToList();
            foreach (Terrain tile in targets)
            {
                if (tile.IsVisible)
                    Game.OverlayHandler.Set(tile.X, tile.Y, Swatch.DbBlood);
            }

            if (!mouse.GetLeftClick())
                return null;

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
