using Optional;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using System;
using System.Linq;

namespace Roguelike.State
{
    internal sealed class AutoexploreState : IState
    {
        private static readonly Lazy<AutoexploreState> _instance = new Lazy<AutoexploreState>(() => new AutoexploreState());
        public static AutoexploreState Instance => _instance.Value;

        private AutoexploreState() { }

        public Option<ICommand> HandleKeyInput(int key)
        {
            bool breaking = false;
            foreach (Tile tile in Game.Map.Discovered)
            {
                Game.Map.GetExit(tile.X, tile.Y).MatchSome(exit => {
                    Game.MessageHandler.AddMessage($"You see an exit to {exit.Destination}");
                    Game.StateHandler.PopState();
                    breaking = true;
                });

                if (breaking)
                    return Option.None<ICommand>();

                Game.Map.GetStack(tile.X, tile.Y).MatchSome(stack =>
                {
                    Game.MessageHandler.AddMessage($"You see {stack}");
                    Game.StateHandler.PopState();
                    breaking = true;
                });

                if (breaking)
                    return Option.None<ICommand>();
            }

            breaking = false;
            foreach (Tile tile in Game.Map.Field.Where(t => t.IsVisible))
            {
                Game.Map.GetActor(tile.X, tile.Y)
                    .Filter(actor => !(actor is Player))
                    .MatchSome(actor =>
                    {
                        Game.MessageHandler.AddMessage($"You see a {actor.Name}");
                        Game.StateHandler.PopState();
                        breaking = true;
                    });

                if (breaking)
                    return Option.None<ICommand>();
            }

            WeightedPoint move = Game.Map.MoveTowardsTarget(
                Game.Player.X, Game.Player.Y, Game.Map.AutoexploreMap);

            if (move.X == Game.Player.X && move.Y == Game.Player.Y)
            {
                // If the best move is to stay still, we must have explored everything reachable
                // already, so we can end autoexplore.
                Game.MessageHandler.AddMessage($"Fully explored {Game.World.CurrentLevel}");
                Game.StateHandler.PopState();
                return Option.None<ICommand>();
            }
            else
            {
                return Option.Some<ICommand>(new MoveCommand(Game.Player, move.X, move.Y));
            }
        }

        public Option<ICommand> HandleMouseInput(int x, int y, bool leftClick, bool rightClick)
        {
            return Option.None<ICommand>();
        }

        public void Update(ICommand command)
        {
            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
        }

        public void Draw(LayerInfo layer)
        {
            Game.Map.Draw(layer);
        }
    }
}
