using Roguelike.Actors;

namespace Roguelike.Interfaces
{
    // Represents items that can be applied.
    interface IUsable
    {
        // Action to perform.
        ISkill ApplyAction { get; }

        // Perform the action.
        void Apply(Actor actor);
    }
}
