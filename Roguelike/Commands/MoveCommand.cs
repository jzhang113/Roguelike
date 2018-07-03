using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Commands
{
    class MoveCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = Utils.Constants.FULL_TURN;

        private readonly int _newX;
        private readonly int _newY;
        private readonly Terrain _tile;

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

            if (Game.Map.TryGetDoor(_tile.X, _tile.Y, out Door door))
            {
                // HACK: need an open door command
                if (!door.IsOpen)
                {
                    Game.Map.OpenDoor(door);
                    return new RedirectMessage(false, new WaitCommand(120));
                }
            }

            // Check if the destination is already occupied.
            if (Game.Map.TryGetActor(_tile.X, _tile.Y, out Actor target))
            {
                if (target == Source)
                    return new RedirectMessage(false, new WaitCommand(Source));

                IAction attack = Source.Equipment.PrimaryWeapon.GetBasicAttack(_tile.X, _tile.Y);
                // should be safe to ask for tiles since we just supplied a target
                IEnumerable<Terrain> targets = attack.Area.GetTilesInRange(Source);
                return new RedirectMessage(false, new ActionCommand(Source, attack, targets));
            }

            return new RedirectMessage(true);
        }

        public void Execute()
        {
            System.Diagnostics.Debug.Assert(_tile.IsWalkable);
            Game.MessageHandler.AddMessage($"{Source.Name} moved to {_newX}, {_newY} and is at {Source.Energy} energy", Enums.MessageLevel.Verbose);

            if (Source is Player)
            {
                // TODO: better handling of move over popups
                if (Game.Map.TryGetStack(_newX, _newY, out InventoryHandler stack))
                {
                    if (!stack.IsEmpty())
                    {
                        Game.MessageHandler.AddMessage(stack.Count == 1
                            ? $"You see {stack.First()} here."
                            : "You see several items here.");
                    }
                }

                if (Game.Map.TryGetExit(_newX, _newY, out Exit exit))
                {
                    Game.MessageHandler.AddMessage($"You see an exit to {exit.Destination}.");
                }
            }

            Game.Map.SetActorPosition(Source, _newX, _newY);
        }
    }
}