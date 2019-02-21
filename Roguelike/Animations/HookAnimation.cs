using BearLib;
using Roguelike.Actors;
using Roguelike.Core;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Animations
{
    internal class HookAnimation : IAnimation
    {
        public LayerInfo Layer { get; }
        public int Turn { get; } = Systems.EventScheduler.Turn;

        private readonly Actor _source;
        private readonly Actor _target;
        private readonly IList<Loc> _path;
        private readonly bool _retract;

        private int _counter;
        private bool _hit;
        private Loc _prevPos;

        public HookAnimation(LayerInfo layer, Actor source, IEnumerable<Loc> path, bool retract, Actor target = null)
        {
            Layer = layer;
            _source = source;
            _path = path.ToList();
            _retract = retract;
            _target = target;

            _counter = 0;
            _hit = false;
        }

        public bool Update()
        {
            if (!_hit)
            {
                // Hook is still extending up to maximum range (_path.Count).
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

                return false;
            }
            else if (_counter <= 0)
            {
                // Hook length is now 0, reactivate Actors.
                _source.DrawingComponent.Activated = true;
                if (_target != null)
                    _target.DrawingComponent.Activated = true;

                return true;
            }
            else
            {
                // Hook is still retracting back.
                _counter -= 2;
                if (_counter < 0)
                    _counter = 0;

                return false;
            }
        }

        public void Draw()
        {
            if (!_hit)
            {
                // Animating the hook going out.
                for (int i = 0; i < _counter; i++)
                {
                    Loc point = _path[i];

                    Terminal.Color(Colors.Hook);
                    Layer.Put(point.X - Camera.X, point.Y - Camera.Y, '~');
                }
            }
            else if (_retract)
            {
                // Animating the hook pulling the _target.
                for (int i = 0; i < _counter - 1; i++)
                {
                    Loc point = _path[i];

                    Terminal.Color(Colors.Hook);
                    Layer.Put(point.X - Camera.X, point.Y - Camera.Y, '~');
                }

                if (_target != null && _counter > 0)
                {
                    _prevPos = _path[_counter - 1];
                    _target.DrawingComponent.Draw(Layer, Game.Map.Field[_prevPos]);
                }
            }
            else
            {
                // Animating the hook pulling the _source along.
                for (int i = _path.Count - 1; i > _path.Count - _counter; i--)
                {
                    Loc point = _path[i];

                    Terminal.Color(Colors.Hook);
                    Layer.Put(point.X - Camera.X, point.Y - Camera.Y, '~');
                }

                if (_counter > 1)
                {
                    _prevPos = _path[_path.Count - _counter + 1];
                    _source.DrawingComponent.Draw(Layer, Game.Map.Field[_prevPos]);
                }
            }
        }
    }
}
