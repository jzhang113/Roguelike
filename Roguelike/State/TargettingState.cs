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
        private int _keyX;
        private int _keyY;
        private bool _useKeyTargetting;

        public TargettingState(Actor source, TargetZone zone, Func<IEnumerable<Terrain>, ICommand> callback)
        {
            _source = source;
            _targetZone = zone;
            _callback = callback;
            _targettableActors = new List<Actor>();

            Game.OverlayHandler.DisplayText = "targetting mode";
            Game.OverlayHandler.ClearBackground();

            ICollection<Terrain> tempRange = new HashSet<Terrain>();
            _inRange = Game.Map.GetTilesInRadius(_source.X, _source.Y, (int)_targetZone.Range).ToList();

            // Filter the targettable range down to only the tiles we have a direct line on.
            foreach (Terrain tile in _inRange)
            {
                foreach (Terrain pathTile in Game.Map.GetStraightLinePath(_source.X, _source.Y, tile.X, tile.Y))
                {
                    Game.OverlayHandler.Set(pathTile.X, pathTile.Y, Swatch.DbGrass, true);
                    tempRange.Add(pathTile);

                    if (!pathTile.IsWalkable)
                        break;
                }
            }

            foreach (Terrain tile in tempRange)
            {
                if (Game.Map.TryGetActor(tile.X, tile.Y, out Actor actor))
                    _targettableActors.Add(actor);
            }

            // Add the current tile into the targettable range as well.
            tempRange.Add(Game.Map.Field[source.X, source.Y]);
            Game.OverlayHandler.Set(source.X, source.Y, Swatch.DbGrass, true);
            _inRange = tempRange;

            Actor firstActor = _targettableActors.FirstOrDefault();
            if (firstActor == null)
            {
                _keyX = source.X;
                _keyY = source.Y;
            }
            else
            {
                _keyX = firstActor.X;
                _keyY = firstActor.Y;
            }

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
                case TargettingInput.MoveW:
                    if (_inRange.Contains(Game.Map.Field[_keyX - 1, _keyY]))
                        _keyX--;
                    break;
                case TargettingInput.MoveS:
                    if (_inRange.Contains(Game.Map.Field[_keyX, _keyY + 1]))
                        _keyY++;
                    break;
                case TargettingInput.MoveN:
                    if (_inRange.Contains(Game.Map.Field[_keyX, _keyY - 1]))
                        _keyY--;
                    break;
                case TargettingInput.MoveE:
                    if (_inRange.Contains(Game.Map.Field[_keyX + 1, _keyY]))
                        _keyX++;
                    break;
                case TargettingInput.MoveNW:
                    if (_inRange.Contains(Game.Map.Field[_keyX - 1, _keyY - 1]))
                    {
                        _keyX--;
                        _keyY--;
                    }
                    break;
                case TargettingInput.MoveNE:
                    if (_inRange.Contains(Game.Map.Field[_keyX + 1, _keyY - 1]))
                    {
                        _keyX++;
                        _keyY--;
                    }
                    break;
                case TargettingInput.MoveSW:
                    if (_inRange.Contains(Game.Map.Field[_keyX - 1, _keyY + 1]))
                    {
                        _keyX--;
                        _keyY++;
                    }
                    break;
                case TargettingInput.MoveSE:
                    if (_inRange.Contains(Game.Map.Field[_keyX + 1, _keyY + 1]))
                    {
                        _keyX++;
                        _keyY++;
                    }
                    break;
                case TargettingInput.NextActor:
                    if (_targettableActors.Any())
                    {
                        Actor nextActor = _targettableActors[++_index % _targettableActors.Count];
                        _keyX = nextActor.X;
                        _keyY = nextActor.Y;
                    }
                    else
                    {
                        _keyX = _source.X;
                        _keyY = _source.Y;
                    }
                    break;
            }

            _useKeyTargetting = true;
            Game.OverlayHandler.ClearForeground();
            IEnumerable<Terrain> targets = _targetZone.GetTilesInRange(_source, _keyX, _keyY).ToList();

            // Draw in the projectile path if any.
            foreach (Terrain tile in _targetZone.Trail)
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, Swatch.DbSun);
            }

            foreach (Terrain tile in targets)
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, Swatch.DbBlood);
            }

            if (input == TargettingInput.Fire)
                return _callback(targets);

            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            if (!MouseInput.GetHoverPosition(mouse, out (int X, int Y) hover))
                return null;

            if (_useKeyTargetting)
                return null;

            int targetX = _source.X;
            int targetY = _source.Y;
            _useKeyTargetting = false;

            // Walk towards the cursor, stopping when we're out of range to get the closest targettable tile.
            foreach (Terrain highlight in Game.Map.GetStraightLinePath(_source.X, _source.Y,
                hover.X, hover.Y))
            {
                if (!_inRange.Contains(highlight))
                    break;

                targetX = highlight.X;
                targetY = highlight.Y;
            }

            Game.OverlayHandler.ClearForeground();
            IEnumerable<Terrain> targets = _targetZone.GetTilesInRange(_source, targetX, targetY).ToList();

            // Draw in the projectile path if any.
            foreach (Terrain tile in _targetZone.Trail)
            {
                Game.OverlayHandler.Set(tile.X, tile.Y, Swatch.DbSun);
            }

            foreach (Terrain tile in targets)
            {
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
