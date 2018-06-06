using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class MoveCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 120;

        private readonly int _newX;
        private readonly int _newY;
        private Terrain _tile;

        public MoveCommand(Actor source, int x, int y)
        {
            Source = source;
            _newX = x;
            _newY = y;
            _tile = Game.Map.Field[_newX, _newY];
        }

        public RedirectMessage Validate()
        {
            // Cancel out of bound moves.
            if (!Game.Map.Field.IsValid(_newX, _newY))
                return new RedirectMessage(false, new WaitCommand(Source));

            // Don't walk into walls.
            if (_tile.IsWall)
            {
                // Don't penalize the player for walking into walls, but monsters should wait if 
                // they will walk into a wall.
                if (Source is Player)
                    return new RedirectMessage(false);
                else
                    return new RedirectMessage(false, new WaitCommand(Source));
            }

            // Check if the destination is already occupied.
            if (_tile.IsOccupied)
            {
                Actor target = Game.Map.GetActor(_tile.X, _tile.Y);

                if (target == Source)
                    return new RedirectMessage(false, new WaitCommand(Source));
                else
                    return new RedirectMessage(false, new AttackCommand(Source, Source.Equipment.PrimaryWeapon.GetBasicAttack(_tile.X, _tile.Y)));
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_tile.IsWalkable);
            Game.MessageHandler.AddMessage($"{Source.Name} moved to {_newX}, {_newY} and is at {Source.Energy} energy", Enums.MessageLevel.Verbose);

            if (Source is Player)
            {
                InventoryHandler itemStack = Game.Map.Field[_newX, _newY].ItemStack;

                if (itemStack != null && !itemStack.IsEmpty())
                {
                    if (itemStack.Size() == 1)
                        Game.MessageHandler.AddMessage($"You see a {itemStack.GetItem('a').Name} here.");
                    else
                        Game.MessageHandler.AddMessage("You see several items here.");
                }
            }

            Game.Map.SetActorPosition(Source, _newX, _newY);
        }
    }
}