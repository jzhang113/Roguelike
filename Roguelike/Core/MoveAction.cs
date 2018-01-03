using Roguelike.Interfaces;
using RogueSharp;

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

        public void Execute()
        {
            if (_newX >= Game.Config.Map.Width || _newY >= Game.Config.Map.Height)
                return;

            Cell cell = Game.Map.GetCell(_newX, _newY);

            if (cell.IsWalkable)
            {
                Game.MessageHandler.AddMessage(string.Format("{0} moved to {1}, {2}", Source.Name, _newX, _newY));
                Game.Map.SetActorPosition(Source, _newX, _newY);
            }
            else
            {
                Game.EventScheduler.Schedule(new AttackAction(Source, Game.Map.GetActor(cell), Source.BasicAttack));
            }
        }
    }
}