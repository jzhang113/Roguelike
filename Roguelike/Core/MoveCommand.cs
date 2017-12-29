using Roguelike.Interfaces;
using RogueSharp;

namespace Roguelike.Core
{
    class MoveCommand : ICommand
    {
        private int dx;
        private int dy;

        public MoveCommand(int dx, int dy)
        {
            this.dx = dx;
            this.dy = dy;
        }

        public IAction Execute(Actor source, Actor target)
        {
            int newX = source.X + dx;
            int newY = source.Y + dy;
            Cell newPos = Game.Map.GetCell(newX, newY);

            if (newPos.IsWalkable)
            {
                Game.MessageHandler.AddMessage(string.Format("{0} moved to {1}, {2}", source.Name, source.X, source.Y));
                return new MoveAction(Game.Player, source.X + dx, source.Y + dy);
            }
            else
            {
                target = Game.Map.GetActor(newPos);

                if (target != null)
                    Game.MessageHandler.AddMessage(source.Name + " attacked " + target.Name);

                return new AttackAction(source, target, 20, 10);
            }
        }
    }
}
