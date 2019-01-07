using System;
using Roguelike.Commands;

namespace Roguelike.Interfaces
{
    // Describes things that may be scheduled by the EventScheduler, such as Actors or Effects.
    public interface ISchedulable : IComparable<ISchedulable>
    {
        // Name used when observed and for the message log.
        string Name { get; }

        // How much energy the entity has. May act when energy > 0.
        int Energy { get; set; }

        // How much energy an entity must be at before it can act. This adds a delay to an entity's
        // first activation, which doesn't matter for persisting entities, but does affect one-shot
        // delayed effects.
        int ActivationEnergy { get; }

        // How many more turns an entity will exist for before expiring. If entities do not expire,
        // they have lifetime -1.
        int Lifetime { get; }

        // The Action to be scheduled.
        ICommand Act();
    }
}
