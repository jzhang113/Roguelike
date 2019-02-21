using Optional;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;

namespace Roguelike.Actions
{
    // Actions are similar to commands, but takes a target and will always trigger when called.
    // Actions may be chained into action sequences, which is considered as a single move and 
    // has a total time cost.
    public interface IAction
    {
        // Holds the area that an Action can target.
        TargetZone Area { get; }

        // Energy cost to perform
        int EnergyCost { get; }

        // Delay before activation
        int Speed { get; }

        // Any visual effects to display.
        Option<IAnimation> Animation { get; }

        // Execute the Action. This takes a Terrain as it may have additional environmental effects.
        void Activate(ISchedulable source, Loc target);
    }
}
