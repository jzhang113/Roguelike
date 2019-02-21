using BearLib;
using Roguelike.Actors;
using Roguelike.Core;

namespace Roguelike.Animations
{
    internal class MoveAnimation : IAnimation
    {
        public LayerInfo Layer { get; }
        public int Turn { get; } = Systems.EventScheduler.Turn;

        private const int _MAX_FRAME = 4;

        private readonly Actor _source;
        private readonly Loc _prev;

        private readonly int _dx;
        private readonly int _dy;
        private int _frame;

        public MoveAnimation(LayerInfo layer, Actor source, Loc prev)
        {
            Layer = layer;
            _source = source;
            _prev = prev;

            _dx = source.Loc.X - prev.X;
            _dy = source.Loc.Y - prev.Y;
            _frame = 0;

            _source.DrawingComponent.Activated = false;
        }

        public bool Update()
        {
            if (_frame >= _MAX_FRAME)
            {
                _source.DrawingComponent.Activated = true;
                return true;
            }
            else
            {
                _frame++;
                return false;
            }
        }

        public void Draw()
        {
            double moveFrac = (double)_frame / _MAX_FRAME;
            int xFrac = (int)(_dx * moveFrac * Terminal.State(Terminal.TK_CELL_WIDTH));
            int yFrac = (int)(_dy * moveFrac * Terminal.State(Terminal.TK_CELL_HEIGHT));

            Terminal.Color(_source.DrawingComponent.Color);
            Terminal.Layer(Layer.Z + 1);
            Terminal.PutExt(
                Layer.X + _prev.X - Camera.X,
                Layer.Y + _prev.Y - Camera.Y,
                xFrac, yFrac, _source.DrawingComponent.Symbol);
            Terminal.Layer(Layer.Z);
        }
    }
}
