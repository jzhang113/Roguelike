using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Systems;
using Roguelike.World;

namespace Roguelike.Commands
{
    class ChangeLevelCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;
        public IAnimation Animation { get; } = null;

        private readonly LevelId _newLevel;

        public ChangeLevelCommand(Actor actor, LevelId newLevel)
        {
            Source = actor;
            _newLevel = newLevel;
        }

        public RedirectMessage Validate()
        {
            if (Game.World.IsValidLevel(_newLevel))
                return new RedirectMessage(true);
            else
                return new RedirectMessage(false);
        }

        public void Execute()
        {
            Game.World.ChangeLevel(_newLevel);
        }
    }
}
