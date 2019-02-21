using Optional;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Statuses;
using Roguelike.Systems;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Commands
{
    internal class MoveCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost => Data.Constants.FULL_TURN;
        public Option<IAnimation> Animation { get; private set; }

        private readonly Loc _nextPos;

        public MoveCommand(Actor source, int x, int y)
        {
            Source = source;
            _nextPos = new Loc(x, y);
        }

        public RedirectMessage Validate()
        {
            // Cancel out of bound moves.
            if (!Game.Map.Field.IsValid(_nextPos))
                return new RedirectMessage(false, new WaitCommand(Source));

            // Don't walk into walls, unless the Actor is currently phasing or we are already
            // inside a wall (to prevent getting stuck).
            if (Game.Map.Field[_nextPos].IsWall
                && !Source.StatusHandler.TryGetStatus(StatusType.Phasing, out _)
                && !Game.Map.Field[Source.X, Source.Y].IsWall)
            {
                // Don't penalize the player for walking into walls, but monsters should wait if 
                // they will walk into a wall.
                if (Source is Player)
                    return new RedirectMessage(false);
                else
                    return new RedirectMessage(false, new WaitCommand(Source));
            }

            if (Game.Map.TryGetDoor(_nextPos.X, _nextPos.Y, out Door door))
            {
                // HACK: need an open door command
                if (!door.IsOpen)
                {
                    door.Open();
                    return new RedirectMessage(false, new WaitCommand(door, 120));
                }
            }

            // Check if the destination is already occupied.
            return Game.Map.GetActor(_nextPos.X, _nextPos.Y).Match(
                some: target =>
                {
                    if (target == Source)
                        return new RedirectMessage(false, new WaitCommand(Source));

                    IAction attack = Source.GetBasicAttack();
                    IEnumerable<Loc> targets = attack.Area.GetTilesInRange(Source, _nextPos);
                    return new RedirectMessage(false, new DelayActionCommand(Source, attack, targets));
                },
                none: () => new RedirectMessage(true));
        }

        public void Execute()
        {
            Game.MessageHandler.AddMessage(
                $"{Source.Name} moved to {_nextPos.X}, {_nextPos.Y} and is at {Source.Energy} energy",
                MessageLevel.Verbose);

            Source.Facing = Utils.Distance.GetNearestDirection(_nextPos, Source.Loc);

            if (Source is IEquipped equipped)
                equipped.Equipment.PrimaryWeapon?.AttackReset();

            if (Source is Player)
            {
                // TODO: better handling of move over popups
                Game.Map.GetStack(_nextPos.X, _nextPos.Y).MatchSome(stack =>
                    Game.MessageHandler.AddMessage(stack.Count == 1
                        ? $"You see {stack.First()} here."
                        : "You see several items here."));

                Game.Map.GetExit(_nextPos.X, _nextPos.Y)
                    .MatchSome(exit => Game.MessageHandler.AddMessage($"You see an exit to {exit.Destination}."));
            }

            int prevX = Source.X;
            int prevY = Source.Y;
            Game.Map.SetActorPosition(Source, _nextPos.X, _nextPos.Y);
            Animation = Option.Some<IAnimation>(new MoveAnimation(Game.StateHandler.CurrentLayer, Source, prevX, prevY));

            if (Source is Player)
                Game.Map.Refresh();
        }
    }
}