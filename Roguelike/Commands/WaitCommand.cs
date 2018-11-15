using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    internal class WaitCommand : ICommand
    {
        public ISchedulable Source { get; }
        public int EnergyCost { get; }
        public IAnimation Animation => null;

        public WaitCommand(ISchedulable source)
        {
            Source = source;
            EnergyCost = source.RefreshRate;
        }

        public WaitCommand(ISchedulable source, int waitTime)
        {
            Source = source;
            EnergyCost = waitTime;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            // whoops, letting everyone refresh the map trashes performance
            if (Source is Player)
                Game.Map.Refresh();
        }
    }
}
