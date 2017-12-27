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

        public void Execute(Actor origin, Actor target)
        {
            int newX = origin.X + dx;
            int newY = origin.Y + dy;
            Cell newPos = Game.Map.GetCell(newX, newY);

            if (newPos.IsWalkable)
            {
                Game.Map.SetActorPosition(origin, origin.X + dx, origin.Y + dy);
            }
            else
            {
                AttackCommand attack = new AttackCommand(origin.BasicAttack);
                attack.Execute(origin, Game.Map.GetActor(newPos));
            }
        }

        public string Message(Actor origin)
        {
            return string.Format("{0} moved to {1}, {2}", origin.Name, origin.X, origin.Y);
        }
    }
}
