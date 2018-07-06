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

        private Actor _source;
        private Actor _target;
        private IList<Terrain> _path;
        private bool _retract;
        private int _counter;
        private bool _hit;
        private Terrain _prevPos;

        public HookAnimation(Actor source, IEnumerable<Terrain> path, bool retract, Actor target = null)
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
                }
            }
            else
            {
                _counter -= 2;
                if (--_counter <= 0)
                {
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
                    Terrain tile = _path[i];
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
                        Terrain tile = _path[i];
                        Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                    }

                    if (_target != null && _counter > 0)
                    {
                        // seems like the easiest way to animate the character moving is to physically move them
                        _prevPos = _path[_counter - 1];
                        Game.Map.SetActorPosition(_target, _prevPos.X, _prevPos.Y);
                    }
                }
                else
                {
                    // Animating the hook pulling the _source along.
                    for (int i = _path.Count - 1; i > _path.Count - _counter; i--)
                    {
                        Terrain tile = _path[i];
                        Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                    }

                    if (_counter > 0)
                    {
                        _prevPos = _path[_path.Count - _counter];
                        Game.Map.SetActorPosition(_source, _prevPos.X, _prevPos.Y);
                    }
                }
            }
        }

        public event EventHandler Complete;
        protected virtual void OnComplete(EventArgs e) => Complete?.Invoke(this, e);
    }
}
