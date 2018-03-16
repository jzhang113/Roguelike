using Roguelike.Actors;
using Roguelike.Core;

namespace Roguelike.Interfaces
{
    // Actions are similar to commands, but takes a target and will always trigger when called.
    // Actions may be chained into action sequences, which is considered as a single move and 
    // has a total time cost.
    interface IAction
    {
        TargetZone Area { get; }

        // Execute the Action. This takes a Terrain as it may have additional environmental effects.
        void Activate(Actor source, Terrain target);
    }
}
