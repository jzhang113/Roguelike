using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Actions
{
    class MoveCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;

        private int _newX;
        private int _newY;
        private Terrain _cell;

        public MoveCommand(Actor source, int x, int y)
        {
            Source = source;
            _newX = x;
            _newY = y;
            _cell = Game.Map.Field[_newX, _newY];
        }

        public RedirectMessage Validate()
        {
            // Cancel out of bound moves.
            if (!Game.Map.Field.IsValid(_newX, _newY))
                return new RedirectMessage(false, new WaitCommand(Source));

            // Don't walk into walls.
            if (_cell.IsWall)
            {
                // Don't penalize the player for walking into walls, but monsters should wait if 
                // they will walk into a wall.
                if (Source is Player)
                    return new RedirectMessage(false);
                else
                    return new RedirectMessage(false, new WaitCommand(Source));
            }

            // Check if the destination is already occupied.
            if (_cell.IsOccupied)
            {
                Actor target = Game.Map.GetActor(_cell.Position);

                if (target == Source)
                    return new RedirectMessage(false, new WaitCommand(Source));
                else
                    return new RedirectMessage(false, new AttackCommand(Source, _cell, Source.Equipment.PrimaryWeapon.GetBasicAttack()));
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_cell.IsWalkable);
            Game.MessageHandler.AddMessage(string.Format("{0} moved to {1}, {2} and is at {3} energy", Source.Name, _newX, _newY, Source.Energy), OptionHandler.MessageLevel.Verbose);

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
    }
}