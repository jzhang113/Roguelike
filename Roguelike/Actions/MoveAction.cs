using Roguelike.Actors;
using Roguelike.Interfaces;
using RogueSharp;

namespace Roguelike.Core
{
    class MoveAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 100;

        private int _newX;
        private int _newY;

        public MoveAction(Actor source, int x, int y)
        {
            _newX = x;
            _newY = y;
            Source = source;
        }

        public void Execute()
        {
            if (_newX >= Game.Config.Map.Width || _newY >= Game.Config.Map.Height)
                return;

            Cell cell = Game.Map.GetCell(_newX, _newY);

            if (cell.IsWalkable)
            {
                //Game.MessageHandler.AddMessage(string.Format("{0} moved to {1}, {2}", Source.Name, _newX, _newY));

                // TODO 3: Handle checking for items better.
                foreach (Items.Item i in Game.Map.Items)
                {
                    if (i.X == _newX && i.Y == _newY && Source is Player)
                        Game.MessageHandler.AddMessage(string.Format("You see a {0} here.", i.Name));
                }

                Game.Map.SetActorPosition(Source, _newX, _newY);
            }
            else
            {
                if (_newX == Source.X && _newY == Source.Y && Source is Player)
                    Game.Map.UpdatePlayerMaps();

                Actor target = Game.Map.GetActor(cell);

                if (target != null)
                    new AttackAction(Source, target, Source.BasicAttack).Execute();
            }
        }
    }
}