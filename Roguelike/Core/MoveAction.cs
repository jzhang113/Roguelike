using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class MoveAction : IAction
    {
        private int _newX;
        private int _newY;
        private Actor _actor;

        public int Time { get; set; }

        public MoveAction(Actor source, int x, int y)
        {
            _newX = x;
            _newY = y;
            _actor = source;
        }

        public void Execute()
        {
            Game.Map.SetActorPosition(_actor, _newX, _newY);
        }
    }
}