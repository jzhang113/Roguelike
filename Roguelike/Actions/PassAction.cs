using Roguelike.Actors;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Actions
{
    class PassAction : IAction
    {
        public Actor Source { get; }
        public int EnergyCost { get; }

        public PassAction(Actor source)
        {
            Source = source;
            EnergyCost = source.RefreshRate;
        }

        public PassAction(int waitTime)
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
