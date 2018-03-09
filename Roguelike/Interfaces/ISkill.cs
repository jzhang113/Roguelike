using Roguelike.Actors;
using Roguelike.Core;

namespace Roguelike.Interfaces
{
    // Skills are similar to actions, but takes a target and will always trigger when called. Skills
    // do not have individual time costs as they can be chained. The entire chain is considered to
    // be one move and has a total time cost.
    public interface ISkill
    {
        // Execute the Action. This takes a Terrain as it may have additional environmental effects.
        void Activate(Terrain target);
    }
}
