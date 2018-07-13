using RLNET;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Input;
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
        private readonly IList<Actor> _targettableActors;

        private int _index;
        private int _targetX;
        private int _targetY;
        private int _prevMouseX;
        private int _prevMouseY;

        public TargettingState(Actor source, TargetZone zone, Func<IEnumerable<Terrain>, ICommand> callback)
        {
            _source = source;
            _targetZone = zone;
            _callback = callback;
            _targettableActors = new List<Actor>();

            Game.OverlayHandler.DisplayText = "targetting mode";
            Game.OverlayHandler.ClearBackground();
            Game.OverlayHandler.ClearForeground();

            ICollection<Terrain> tempRange = new HashSet<Terrain>();
            _inRange = Game.Map.GetTilesInRadius(_source.X, _source.Y, (int)_targetZone.Range).ToList();

            // Filter the targettable range down to only the tiles we have a direct line on.
            foreach (Terrain tile in _inRange)
            {
                Terrain collision = tile;
                foreach (var current in Game.Map.GetStraightLinePath(_source.X, _source.Y, tile.X, tile.Y))
                {
                    if (!current.IsWalkable)
                    {
                        collision = current;
                        break;
                    }
                }

                Game.OverlayHandler.Set(collision.X, collision.Y, Swatch.DbGrass, true);
                tempRange.Add(collision);
            }

            // Pick out the interesting targets.
            // TODO: select items for item targetting spells
            foreach (Terrain tile in tempRange)
            {
                if (Game.Map.TryGetActor(tile.X, tile.Y, out Actor actor))
                    _targettableActors.Add(actor);
            }

            // Add the current tile into the targettable range as well.
            tempRange.Add(Game.Map.Field[source.X, source.Y]);
            Game.OverlayHandler.Set(source.X, source.Y, Swatch.DbGrass, true);
            _inRange = tempRange;

            // Initialize the targetting to an interesting target.
            Actor firstActor = _targettableActors.FirstOrDefault();
            if (firstActor == null)
            {
                _targetX = source.X;
                _targetY = source.Y;
            }
            else
            {
                _targetX = firstActor.X;
                _targetY = firstActor.Y;
            }
            DrawTargettedTiles();

            // Initialize the saved mouse position as well.
            _prevMouseX = _targetX;
            _prevMouseY = _targetY;

            Game.ForceRender();
        }

        // ReSharper disable once CyclomaticComplexity
        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            TargettingInput input = InputMapping.GetTargettingInput(keyPress);
            switch (input)
            {
                case TargettingInput.None:
                    return null;
                case TargettingInput.JumpW:
                    JumpTarget(Direction.W);
                    break;
                case TargettingInput.JumpS:
                    JumpTarget(Direction.S);
                    break;
                case TargettingInput.JumpN:
                    JumpTarget(Direction.N);
                    break;
                case TargettingInput.JumpE:
                    JumpTarget(Direction.E);
                    break;
                case TargettingInput.JumpNW:
                    JumpTarget(Direction.NW);
                    break;
                case TargettingInput.JumpNE:
                    JumpTarget(Direction.NE);
                    break;
                case TargettingInput.JumpSW:
                    JumpTarget(Direction.SW);
                    break;
                case TargettingInput.JumpSE:
                    JumpTarget(Direction.SE);
                    break;
                case TargettingInput.MoveW:
                    MoveTarget(Direction.W);
                    break;
                case TargettingInput.MoveS:
                    MoveTarget(Direction.S);
                    break;
                case TargettingInput.MoveN:
                    MoveTarget(Direction.N);
                    break;
                case TargettingInput.MoveE:
                    MoveTarget(Direction.E);
                    break;
                case TargettingInput.MoveNW:
                    MoveTarget(Direction.N);
                    MoveTarget(Direction.W);
                    break;
                case TargettingInput.MoveNE:
                    MoveTarget(Direction.N);
                    MoveTarget(Direction.E);
                    break;
                case TargettingInput.MoveSW:
                    MoveTarget(Direction.S);
                    MoveTarget(Direction.W);
                    break;
                case TargettingInput.MoveSE:
                    MoveTarget(Direction.S);
                    MoveTarget(Direction.E);
                    break;
                case TargettingInput.NextActor:
                    if (_targettableActors.Any())
                    {
                        Actor nextActor = _targettableActors[++_index % _targettableActors.Count];
                        _targetX = nextActor.X;
                        _targetY = nextActor.Y;
                    }
                    else
                    {
                        _targetX = _source.X;
                        _targetY = _source.Y;
                    }
                    break;
            }

            IEnumerable<Terrain> targets = DrawTargettedTiles();
            return input == TargettingInput.Fire ? _callback(targets) : null;
        }

        private void MoveTarget(WeightedPoint direction)
        {
            if (Game.Map.Field.IsValid(_targetX + direction.X, _targetY + direction.Y)
                && _inRange.Contains(Game.Map.Field[_targetX + direction.X, _targetY + direction.Y]))
            {
                _targetX += direction.X;
                _targetY += direction.Y;
            }
        }

        private void JumpTarget(WeightedPoint direction)
        {
            while (Game.Map.Field.IsValid(_targetX + direction.X, _targetY + direction.Y)
                   && _inRange.Contains(Game.Map.Field[_targetX + direction.X, _targetY + direction.Y]))
            {
                _targetX += direction.X;
                _targetY += direction.Y;
            }
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            if (!MouseInput.GetHoverPosition(mouse, out (int X, int Y) hover))
                return null;

            // If the mouse didn't get moved, don't do anything.
            if (hover.X == _prevMouseX && hover.Y == _prevMouseY)
                return null;

            _prevMouseX = hover.X;
            _prevMouseY = hover.Y;

            // Adjust the targetting position so it remains in the targetting range.
            _targetX = _source.X;
            _targetY = _source.Y;

            foreach (Terrain highlight in Game.Map.GetStraightLinePath(_source.X, _source.Y,
                _prevMouseX, _prevMouseY))
            {
                if (!_inRange.Contains(highlight))
                    break;

                _targetX = highlight.X;
                _targetY = highlight.Y;
            }

            IEnumerable<Terrain> targets = DrawTargettedTiles();
            return mouse.GetLeftClick() ? _callback(targets) : null;
        }

        private IEnumerable<Terrain> DrawTargettedTiles()
        {
            Game.OverlayHandler.ClearForeground();
            IEnumerable<Terrain> targets = _targetZone.GetTilesInRange(_source, _targetX, _targetY).ToList();

            // Draw in the projectile path if any.
            foreach (Terrain tile in _targetZone.Trail)
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, Swatch.DbSun);
            }

            // Draw the targetted tiles.
            foreach (Terrain tile in targets)
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, Swatch.DbBlood);
            }
            
            // TODO: Replace colors
            Game.OverlayHandler.Set(_targetX, _targetY, new RLColor(255, 0, 0));

            return targets;
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
