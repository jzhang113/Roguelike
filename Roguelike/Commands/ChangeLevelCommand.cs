using Optional;
using Roguelike.Animations;
using Roguelike.Systems;
using Roguelike.World;

namespace Roguelike.Commands
{
    internal class ChangeLevelCommand : ICommand
    {
        public int EnergyCost => 0;
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        private readonly Option<LevelId> _newLevel;

        public ChangeLevelCommand(in Option<LevelId> newLevel)
        {
            _newLevel = newLevel;
        }

        public RedirectMessage Validate() =>
            _newLevel.Match(
                some: level => Game.World.IsValidLevel(level)
                    ? new RedirectMessage(true)
                    : new RedirectMessage(false),
                none: () => new RedirectMessage(false));

        public void Execute() => _newLevel.MatchSome(level => Game.World.ChangeLevel(level));
    }
}
