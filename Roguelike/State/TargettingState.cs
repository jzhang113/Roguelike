using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.State
{
    internal class TargettingState : IState
    {
        public bool Nonblocking => false;

        private readonly Actor _source;
        private readonly TargetZone _targetZone;
        private readonly Func<IEnumerable<Tile>, ICommand> _callback;
        private readonly IEnumerable<Tile> _inRange;
        private readonly IList<Actor> _targettableActors;

        private int _index;
        private int _targetX;
        private int _targetY;
        private int _prevMouseX;
        private int _prevMouseY;

        public TargettingState(Actor source, TargetZone zone, Func<IEnumerable<Tile>, ICommand> callback)
        {
            _source = source;
            _targetZone = zone;
            _callback = callback;
            _targettableActors = new List<Actor>();

            Game.OverlayHandler.DisplayText = "targetting mode";
            Game.OverlayHandler.ClearBackground();
            Game.OverlayHandler.ClearForeground();

            ICollection<Tile> tempRange = new HashSet<Tile>();
            _inRange = Game.Map.GetTilesInRadius(_source.X, _source.Y, (int)_targetZone.Range).ToList();

            // Filter the targettable range down to only the tiles we have a direct line on.
            foreach (Tile tile in _inRange)
            {
                Tile collision = tile;
                foreach (Tile current in
                    Game.Map.GetStraightLinePath(_source.X, _source.Y, tile.X, tile.Y))
                {
                    if (!current.IsWalkable)
                    {
                        collision = current;
                        break;
                    }
                }

                Game.OverlayHandler.Set(collision.X, collision.Y, Colors.TargetBackground, true);
                tempRange.Add(collision);
            }

            // Pick out the interesting targets.
            // TODO: select items for item targetting spells
            foreach (Tile tile in tempRange)
            {
                if (Game.Map.TryGetActor(tile.X, tile.Y, out Actor actor))
                    _targettableActors.Add(actor);
            }

            // Add the current tile into the targettable range as well.
            tempRange.Add(Game.Map.Field[source.X, source.Y]);
            Game.OverlayHandler.Set(source.X, source.Y, Colors.TargetBackground, true);
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
        }

        // ReSharper disable once CyclomaticComplexity
        public ICommand HandleKeyInput(int key)
        {
            switch (InputMapping.GetTargettingInput(key))
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
                    if (_targettableActors.Count > 0)
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

            IEnumerable<Tile> targets = DrawTargettedTiles();
            return InputMapping.GetTargettingInput(key) == TargettingInput.Fire ? _callback(targets) : null;
        }

        private void MoveTarget(Dir direction)
        {
            (int dx, int dy) = direction;

            if (Game.Map.Field.IsValid(_targetX + dx, _targetY + dy)
                && _inRange.Contains(Game.Map.Field[_targetX + dx, _targetY + dy]))
            {
                _targetX += dx;
                _targetY += dy;
            }
        }

        private void JumpTarget(Dir direction)
        {
            (int dx, int dy) = direction;

            while (Game.Map.Field.IsValid(_targetX + dx, _targetY + dy)
                   && _inRange.Contains(Game.Map.Field[_targetX + dx, _targetY + dy]))
            {
                _targetX += dx;
                _targetY += dx;
            }
        }

        public ICommand HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            // Handle clicks before checking for movement.
            if (leftClick)
            {
                IEnumerable<Tile> targets = DrawTargettedTiles();
                return _callback(targets);
            }

            int adjustedX = x + Camera.X;
            int adjustedY = y + Camera.Y;

            // If the mouse didn't get moved, don't do anything.
            if (adjustedX == _prevMouseX && adjustedY == _prevMouseY)
                return null;

            _prevMouseX = adjustedX;
            _prevMouseY = adjustedY;

            // Adjust the targetting position so it remains in the targetting range.
            _targetX = _source.X;
            _targetY = _source.Y;

            foreach (Tile highlight in
                Game.Map.GetStraightLinePath(_source.X, _source.Y, _prevMouseX, _prevMouseY))
            {
                if (!_inRange.Contains(highlight))
                    break;

                _targetX = highlight.X;
                _targetY = highlight.Y;
            }

            DrawTargettedTiles();
            return null;
        }

        private IEnumerable<Tile> DrawTargettedTiles()
        {
            Game.OverlayHandler.ClearForeground();
            IEnumerable<Tile> targets = _targetZone.GetTilesInRange(_source, _targetX, _targetY).ToList();

            // Draw the projectile path if any.
            foreach (Tile tile in _targetZone.Trail)
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, Colors.Path);
            }

            // Draw the targetted tiles.
            foreach (Tile tile in targets)
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, Colors.Target);
            }

            Game.OverlayHandler.Set(_targetX, _targetY, Colors.Cursor);
            return targets;
        }

        public void Update(ICommand command)
        {
            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
            Game.StateHandler.PopState();

            if (command.Animation != null)
                Game.StateHandler.PushState(new AnimationState(command.Animation));
        }

        public void Draw(LayerInfo layer)
        {
            Game.OverlayHandler.Draw(layer);
            Game.World.Map.Draw(layer);
        }
    }
}
