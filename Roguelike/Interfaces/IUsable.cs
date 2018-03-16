using Roguelike.Actions;
using Roguelike.Core;
using System.Collections.Generic;

namespace Roguelike.Interfaces
{
    // Represents items that can be applied.
    interface IUsable
    {
        // Action to perform.
        ActionSequence ApplySkill { get; }

        // Perform the action.
        void Apply(IEnumerable<Terrain> targets);
    }
}
