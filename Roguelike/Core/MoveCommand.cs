using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class MoveUpCommand : ICommand
    {
        public void Execute(Actor actor)
        {
            Game.DungeonMap.SetActorPosition(actor, actor.X, actor.Y - 1);
        }
    }

    class MoveDownCommand : ICommand
    {
        public void Execute(Actor actor)
        {
            Game.DungeonMap.SetActorPosition(actor, actor.X, actor.Y + 1);
        }
    }

    class MoveLeftCommand : ICommand
    {
        public void Execute(Actor actor)
        {
            Game.DungeonMap.SetActorPosition(actor, actor.X - 1, actor.Y);
        }
    }

    class MoveRightCommand : ICommand
    {
        public void Execute(Actor actor)
        {
            Game.DungeonMap.SetActorPosition(actor, actor.X + 1, actor.Y);
        }
    }
}
