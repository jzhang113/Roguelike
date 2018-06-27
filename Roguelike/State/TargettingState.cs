using RLNET;
using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Systems;

namespace Roguelike.State
{
    class TargettingState : IState
    {
        private readonly ITargetCommand _targetCommand;
        private readonly Actor _targetSource;
        private readonly IAction _targetAction;

        public TargettingState(ITargetCommand command, Actor source, IAction action)
        {
            _targetCommand = command;
            _targetSource = source;
            _targetAction = action;
        }

        public ICommand HandleKeyInput(RLKeyPress keyPress)
        {
            // TODO
            return null;
        }

        public ICommand HandleMouseInput(RLMouse mouse)
        {
            foreach (Terrain tile in Game.Map.GetTilesInRadius(_targetSource.X, _targetSource.Y, (int)_targetAction.Area.Range))
            {
                Game.Map.Highlight[tile.X, tile.Y] = Swatch.DbGrass;
            }

            if (!MouseHandler.GetClickPosition(mouse, out (int X, int Y) click))
                return null;

            int distance = Utils.Distance.EuclideanDistanceSquared(_targetSource.X, _targetSource.Y, click.X, click.Y);
            float maxRange = _targetAction.Area.Range * _targetAction.Area.Range;

            if (distance > maxRange)
            {
                Game.MessageHandler.AddMessage("Target out of range.");
                return null;
            }

            _targetCommand.Target = _targetAction.Area.GetTilesInRange(_targetSource, click);
            return _targetCommand;
        }

        public void Update()
        {
            Game.ForceRender();
            ICommand command = Game.StateHandler.HandleInput();
            if (command == null)
                return;

            if (EventScheduler.Execute(Game.Player, command))
                Game.StateHandler.PopState();
        }

        public void Draw()
        {
            Game.MapConsole.Print(1, 1, "targetting mode", Colors.TextHeading);

            //IEnumerable<Terrain> path = Game.Map.GetStraightLinePath(Game.Player.X, Game.Player.Y, mousePos.X, mousePos.Y);
            //foreach (Terrain tile in path)
            //{
            //    if (!Game.Map.Field[tile.X, tile.Y].IsExplored)
            //    {
            //        break;
            //    }

            //    Game.Map.Highlight[tile.X, tile.Y] = RLColor.Red;
            //}
        }
    }
}
