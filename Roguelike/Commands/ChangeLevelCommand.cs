using Roguelike.Actors;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    class ChangeLevelCommand : ICommand
    {
        public Actor Source { get; }
        public int EnergyCost { get; } = 0;

        private string _newLevel;

        public ChangeLevelCommand(Actor actor, string newLevel)
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
