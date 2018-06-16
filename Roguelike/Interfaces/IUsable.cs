using Roguelike.Core;
using System.Collections.Generic;
using Roguelike.Actions;

namespace Roguelike.Interfaces
{
    // Represents items that can be applied.
    interface IUsable
    {
        // Action to perform.
        IAction ApplySkill { get; }

        // Perform the action.
        void Apply(IEnumerable<Terrain> targets);
    }
}
