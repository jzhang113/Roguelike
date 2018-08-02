using System;
using System.Linq;
using RLNET;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;

namespace Roguelike.State
{
    class AutoexploreState : IState
    {
        private static readonly Lazy<AutoexploreState> _instance = new Lazy<AutoexploreState>(() => new AutoexploreState());
        public static AutoexploreState Instance => _instance.Value;

        private AutoexploreState()
        {
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            foreach (Tile tile in Game.Map.Discovered)
            {
                if (Game.Map.TryGetExit(tile.X, tile.Y, out Exit exit))
                {
                    Game.MessageHandler.AddMessage($"You see an exit to {exit.Destination}");
                    Game.StateHandler.PopState();
                    return null;
                }

                if (Game.Map.TryGetStack(tile.X, tile.Y, out Systems.InventoryHandler stack))
                {
                    Game.MessageHandler.AddMessage($"You see {stack}");
                    Game.StateHandler.PopState();
                    return null;
                }
            }

            Actor actor = null;
            if (Game.Map.Field.Where(tile => tile.IsVisible)
                .Any(tile => Game.Map.TryGetActor(tile.X, tile.Y, out actor) && !(actor is Player)))
            {
                Game.MessageHandler.AddMessage($"You see a {actor.Name}");
                Game.StateHandler.PopState();
                return null;
            }

            WeightedPoint move = Game.Map.MoveTowardsTarget(
                Game.Player.X, Game.Player.Y, Game.Map.AutoexploreMap);
            if (move.X == Game.Player.X && move.Y == Game.Player.Y)
            {
                // If the best move is to stay still, we must have explored everything reachable
                // already, so we can end autoexplore.
                Game.MessageHandler.AddMessage($"Fully explored {Game.World.CurrentLevel}");
                Game.StateHandler.PopState();
                return null;
            }
            else
            {
                return new MoveCommand(Game.Player, move.X, move.Y);
            }
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            return null;
        }

        public void Update()
        {
            ICommand command = Game.StateHandler.HandleInput();
            if (command == null)
                return;

            Game.Player.NextCommand = command;
            Game.EventScheduler.Run();
            Game.ForceRender();
        }

        public void Draw(RLConsole mapConsole)
        {
            mapConsole.Clear(0, RLColor.Black, Colors.Text);
            Game.Map.Draw(mapConsole);
        }
    }
}
