using Roguelike.Actors;

namespace Roguelike.Interfaces
{
    public interface IAction
    {
        Actor Source { get; }
        int EnergyCost { get; }
        
        void Execute();
    }
}
