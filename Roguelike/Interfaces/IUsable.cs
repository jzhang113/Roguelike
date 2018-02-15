using Roguelike.Actors;
using Roguelike.Skills;
using System;

namespace Roguelike.Interfaces
{
    // Represents items that can be applied.
    interface IUsable
    {
        // Action to perform.
        Skill ApplySkill { get; }

        // Perform the action.
        void Apply();
    }
}
