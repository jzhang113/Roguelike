using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;
using RogueSharp;

namespace Roguelike.Core
{
    class MoveAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 100;

        private int _newX;
        private int _newY;
        private Cell _cell;

        public MoveAction(Actor source, int x, int y)
        {
            Source = source;
            _newX = x;
            _newY = y;
            _cell = Game.Map.GetCell(_newX, _newY);
        }

        public RedirectMessage Validate()
        {
            // Cancel out of bound moves.
            if (_newX >= Game.Config.Map.Width || _newY >= Game.Config.Map.Height)
                return new RedirectMessage(false);

            // Check if the destination is already occupied.
            Actor target = Game.Map.GetActor(_cell);
            if (target != null)
                return new RedirectMessage(false, new AttackAction(Source, target, Source.BasicAttack));

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            if (_cell.IsWalkable)
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
            }
        }
    }
}