using Roguelike.Actors;
using Roguelike.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Animations
{
    class HookAnimation : IAnimation
    {
        public bool Done { get; private set; }

        private readonly Actor _source;
        private readonly Actor _target;
        private readonly IList<Tile> _path;
        private readonly bool _retract;
        private int _counter;
        private bool _hit;
        private Tile _prevPos;

        public HookAnimation(Actor source, IEnumerable<Tile> path, bool retract, Actor target = null)
        {
            _source = source;
            _path = path.ToList();
            _retract = retract;
            _target = target;

            _prevPos = Game.Map.Field[_source.X, _source.Y];
            Done = _path.Count == 0;
        }

        public void Update()
        {
            System.Diagnostics.Debug.Assert(!Done);

            if (!_hit)
            {
                _counter += 2;
                if (_counter > _path.Count - 1)
                {
                    _counter = _path.Count - 1;
                    _hit = true;

                    // Deactivate Actors so duplicates aren't drawn.
                    if (!_retract)
                        _source.DrawingComponent.Activated = false;
                    else if (_target != null)
                        _target.DrawingComponent.Activated = false;
                }
            }
            else
            {
                _counter -= 2;
                if (--_counter <= 0)
                {
                    // Reactivate Actors.
                    _source.DrawingComponent.Activated = true;
                    if (_target != null)
                        _target.DrawingComponent.Activated = true;

                    _counter = 0;
                    Done = true;
                    OnComplete(EventArgs.Empty);
                }
            }
        }

        public void Draw()
        {
            if (!_hit)
            {
                // Animating the hook going out.
                for (int i = 0; i < _counter; i++)
                {
                    Tile tile = _path[i];
                    Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                }
            }
            else
            {
                if (_retract)
                {
                    // Animating the hook pulling the _target.
                    for (int i = 0; i < _counter - 1; i++)
                    {
                        Tile tile = _path[i];
                        Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                    }

                    if (_target != null && _counter > 0)
                    {
                        _prevPos = _path[_counter - 1];
                        _target.DrawingComponent.Draw(Game.MapConsole, _prevPos);
                    }
                }
                else
                {
                    // Animating the hook pulling the _source along.
                    for (int i = _path.Count - 1; i > _path.Count - _counter; i--)
                    {
                        Tile tile = _path[i];
                        Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                    }

                    if (_counter > 1)
                    {
                        _prevPos = _path[_path.Count - _counter + 1];
                        _source.DrawingComponent.Draw(Game.MapConsole, _prevPos);
                    }
                }
            }
        }

        public event EventHandler Complete;
        private void OnComplete(EventArgs e) => Complete?.Invoke(this, e);
    }
}
