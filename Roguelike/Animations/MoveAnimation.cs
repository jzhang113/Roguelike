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
        private readonly int _prevX;
        private readonly int _prevY;

        private readonly int _dx;
        private readonly int _dy;
        private int _frame;

        public MoveAnimation(LayerInfo layer, Actor source, int prevX, int prevY)
        {
            Layer = layer;
            _source = source;
            _prevX = prevX;
            _prevY = prevY;

            _dx = source.X - prevX;
            _dy = source.Y - prevY;
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
                Layer.X + _prevX - Camera.X,
                Layer.Y + _prevY - Camera.Y,
                xFrac, yFrac, _source.DrawingComponent.Symbol);
            Terminal.Layer(Layer.Z);
        }
    }
}
