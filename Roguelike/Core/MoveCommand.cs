using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class MoveCommand : ICommand
    {
        int dx;
        int dy;

        public MoveCommand(int dx, int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }

        public void Execute(Actor origin, Actor target)
        {
            Game.DungeonMap.SetActorPosition(origin, origin.X + dx, origin.Y + dy);
        }

        public string Message(Actor origin)
        {
            return string.Format("{0} moved to {1}, {2}", origin.Name, origin.X, origin.Y);
        }
    }
}
