using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Actions
{
    class WaitAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        public WaitAction(Actor source)
        {
            Source = source;
            EnergyCost = source.RefreshRate;
        }

        public WaitAction(int waitTime)
        {
            EnergyCost = waitTime;
        }

        public RedirectMessage Validate()
        {
            return new RedirectMessage(true);
        }

        public void Execute()
        {
            return;
        }
    }
}
