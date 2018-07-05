using Roguelike.Actors;
using Roguelike.Core;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Animations
{
    class HookAnimation : IAnimation
    {
        public bool Done { get; private set; }

        private Actor _source;
        private IList<Terrain> _path;
        private bool _pull;
        private int _counter;
        private bool _hit;
        private Terrain _prevPos;

        public HookAnimation(Actor source, Terrain target)
        {
            _source = source;
            _path = Game.Map.GetStraightLinePath(source.X, source.Y, target.X, target.Y).ToList();

            Done = _path.Count == 0;
        }

        public void Update()
        {
            System.Diagnostics.Debug.Assert(!Done);

            if (!_hit)
            {
                if (++_counter > _path.Count - 1)
                {
                    _hit = true;
                    _prevPos = Game.Map.Field[_source.X, _source.Y];
                    // _pull = true;
                }
            }
            else
            {
                if (--_counter <= 2)
                    Done = true;
            }
        }

        public void Draw()
        {
            if (!_hit)
            {
                for (int i = 0; i < _counter; i++)
                {
                    Terrain tile = _path[i];
                    Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                }
            }
            else
            {
                if (_pull)
                {
                    for (int i = 0; i < _counter; i++)
                    {
                        Terrain tile = _path[i];
                        Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                    }
                }
                else
                {
                    _source.DrawingComponent.Draw(Game.MapConsole, _prevPos, _prevPos.X - Camera.X, _prevPos.Y - Camera.Y);

                    for (int i = _path.Count - 1; i >= _path.Count - _counter + 1; i--)
                    {
                        Terrain tile = _path[i];
                        Game.MapConsole.Set(tile.X - Camera.X, tile.Y - Camera.Y, Swatch.DbLight, null, '~');
                    }

                    _prevPos = _path[_path.Count - _counter + 1];
                }
            }
        }
    }
}
