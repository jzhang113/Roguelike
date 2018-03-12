using System;

namespace Roguelike.Interfaces
{
    // Describes things that may be scheduled by the EventScheduler, such as Actors or Effects.
    public interface ISchedulable : IComparable<ISchedulable>
    {
        // How much energy the Actor has. Actors may act when energy >= 0. 120 Energy is a standard tick.
        int Energy { get; set; }

        // How much energy the Actor regains per game turn.
        int RefreshRate { get; set; }

        // The Action to be scheduled.
        ICommand Act();
    }
}
