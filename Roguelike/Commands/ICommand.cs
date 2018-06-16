using Roguelike.Actors;
using Roguelike.Systems;

namespace Roguelike.Commands
{
    // Describes most possible in game actions that take time. Actions that do not take time, such
    // as opening the inventory, are not Actions.
    public interface ICommand
    {
        // The Actor performing the Action.
        Actor Source { get; }

        // How long the Action takes to perform.
        int EnergyCost { get; }

        // Check if the Action succeed before execution. Invalid actions should be cancelled and
        // not expend energy. Some invalid actions may be redirected to become valid actions.
        RedirectMessage Validate();

        // Execute the Action. This will immediately reduce the Actor's energy levels by the energy
        // cost.
        void Execute();
    }
}
