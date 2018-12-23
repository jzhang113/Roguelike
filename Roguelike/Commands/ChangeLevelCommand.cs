using Roguelike.Animations;
using Roguelike.Systems;
using Roguelike.World;

namespace Roguelike.Commands
{
    internal class ChangeLevelCommand : ICommand
    {
        public int EnergyCost => 0;
        public IAnimation Animation => null;

        private readonly LevelId _newLevel;

        public ChangeLevelCommand(in LevelId newLevel)
        {
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
