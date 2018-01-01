using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class MoveAction : IAction
    {
        public IActor Source { get; }
        public int Time { get; set; }

        private int _newX;
        private int _newY;

        public MoveAction(IActor source, int x, int y)
        {
            _newX = x;
            _newY = y;
            Source = source;

            Time = source.QueuedTime + 50;
        }

        public void Execute() => Game.Map.SetActorPosition(Source, _newX, _newY);
    }
}