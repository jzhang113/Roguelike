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
                Game.MessageHandler.AddMessage(string.Format("{0} moved to {1}, {2}", Source.Name, _newX, _newY), OptionHandler.MessageLevel.Verbose);

                if (Source is Player)
                {
                    InventoryHandler itemStack = Game.Map.Field[_newX, _newY].ItemStack;

                    if (itemStack != null && !itemStack.IsEmpty())
                    {
                        if (itemStack.Size() == 1)
                            Game.MessageHandler.AddMessage(string.Format("You see a {0} here.", itemStack.GetItem('a').Name), OptionHandler.MessageLevel.Normal);
                        else
                            Game.MessageHandler.AddMessage("You see several items here.", OptionHandler.MessageLevel.Normal);
                    }
                }

                Game.Map.SetActorPosition(Source, _newX, _newY);
            }
            else
            {
                // HACK: Update maps should probably go somewhere nicer...
                if (_newX == Source.X && _newY == Source.Y && Source is Player)
                    Game.Map.UpdatePlayerMaps();
            }
        }
    }
}