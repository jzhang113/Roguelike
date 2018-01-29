using Roguelike.Actors;

namespace Roguelike.Interfaces
{
    interface IAction
    {
        Actor Source { get; }
        int EnergyCost { get; }
        
        void Execute();
    }
}
