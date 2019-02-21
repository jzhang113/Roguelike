using Optional;
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
        private readonly Actor _source;
        private readonly TargetZone _targetZone;
        private readonly Func<IEnumerable<Loc>, ICommand> _callback;
        private readonly IEnumerable<Loc> _inRange;
        private readonly IList<Actor> _targettableActors;

        private int _index;
        private Loc _target;
        private Loc _prevMouse;

        public TargettingState(Actor source, TargetZone zone, Func<IEnumerable<Loc>, ICommand> callback)
        {
            _source = source;
            _targetZone = zone;
            _callback = callback;
            _targettableActors = new List<Actor>();

            Game.Overlay.DisplayText = "targetting mode";
            Game.Overlay.Clear();

            ICollection<Loc> tempRange = new HashSet<Loc>();
            _inRange = Game.Map.GetPointsInRadius(_source.Loc, _targetZone.Range).ToList();

            // Filter the targettable range down to only the tiles we have a direct line on.
            foreach (Loc point in _inRange)
            {
                Loc collision = point;
                foreach (Loc current in Game.Map.GetStraightLinePath(_source.Loc, point))
                {
                    if (!Game.Map.Field[current].IsWalkable)
                    {
                        collision = current;
                        break;
                    }
                }

                Game.Overlay.Set(collision.X, collision.Y, Colors.TargetBackground);
                tempRange.Add(collision);
            }

            // Pick out the interesting targets.
            // TODO: select items for item targetting spells
            foreach (Loc point in tempRange)
            {
                Game.Map.GetActor(point.X, point.Y)
                    .MatchSome(actor => _targettableActors.Add(actor));
            }

            // Add the current tile into the targettable range as well.
            tempRange.Add(source.Loc);
            Game.Overlay.Set(source.X, source.Y, Colors.TargetBackground);
            _inRange = tempRange;

            // Initialize the targetting to an interesting target.
            Actor firstActor = _targettableActors.FirstOrDefault();
            if (firstActor == null)
            {
                _target = source.Loc;
            }
            else
            {
                _target = firstActor.Loc;
            }
            DrawTargettedTiles();

            // Initialize the saved mouse position as well.
            _prevMouse = _target;
        }

        // ReSharper disable once CyclomaticComplexity
        public Option<ICommand> HandleKeyInput(int key)
        {
            switch (InputMapping.GetTargettingInput(key))
            {
                case TargettingInput.None:
                    return Option.None<ICommand>();
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
                        _target = nextActor.Loc;
                    }
                    else
                    {
                        _target = _source.Loc;
                    }
                    break;
            }

            IEnumerable<Loc> targets = DrawTargettedTiles();
            return (InputMapping.GetTargettingInput(key) == TargettingInput.Fire)
                ? Option.Some(_callback(targets))
                : Option.None<ICommand>();
        }

        private void MoveTarget(in Dir direction)
        {
            (int dx, int dy) = direction;
            Loc next = new Loc(_target.X + dx, _target.Y + dy);

            if (Game.Map.Field.IsValid(next) && _inRange.Contains(next))
            {
                _target = next;
            }
        }

        private void JumpTarget(in Dir direction)
        {
            (int dx, int dy) = direction;
            Loc next = new Loc(_target.X + dx, _target.Y + dy);

            while (Game.Map.Field.IsValid(next) && _inRange.Contains(next))
            {
                _target = next;
                next = new Loc(next.X + dx, next.Y + dx);
            }
        }

        public Option<ICommand> HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            // Handle clicks before checking for movement.
            if (leftClick)
            {
                IEnumerable<Loc> targets = DrawTargettedTiles();
                return Option.Some(_callback(targets));
            }

            // If the mouse didn't get moved, don't do anything.
            if (x + Camera.X == _prevMouse.X && y + Camera.Y == _prevMouse.Y)
                return Option.None<ICommand>();

            _prevMouse = new Loc(x + Camera.X, y+ Camera.Y);

            // Adjust the targetting position so it remains in the targetting range.
            _target = _source.Loc;

            foreach (Loc highlight in Game.Map.GetStraightLinePath(_source.Loc, _prevMouse))
            {
                if (!_inRange.Contains(highlight))
                    break;

                _target = highlight;
            }

            DrawTargettedTiles();
            return Option.None<ICommand>();
        }

        private IEnumerable<Loc> DrawTargettedTiles()
        {
            Game.Overlay.Clear();
            IEnumerable<Loc> targets = _targetZone.GetTilesInRange(_source, _target).ToList();

            // Draw the projectile path if any.
            foreach (Loc point in _targetZone.Trail)
            {
                Game.Overlay.Set(point.X, point.Y, Colors.Path);
            }

            // Draw the targetted tiles.
            foreach (Loc point in targets)
            {
                Game.Overlay.Set(point.X, point.Y, Colors.Target);
            }

            Game.Overlay.Set(_target.X, _target.Y, Colors.Cursor);
            return targets;
        }

        public void Update(ICommand command)
        {
            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
            Game.StateHandler.PopState();
        }

        public void Draw(LayerInfo layer)
        {
            Game.Overlay.Draw(layer);
            Game.World.Map.Draw(layer);
        }
    }
}
