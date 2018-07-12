using System;
using Roguelike.Commands;

namespace Roguelike.Interfaces
{
    // Describes things that may be scheduled by the EventScheduler, such as Actors or Effects.
    public interface ISchedulable : IComparable<ISchedulable>
    {
        // How much energy the Actor has. Actors may act when energy >= 0.
        int Energy { get; set; }

        // How much energy the Actor regains per game turn. 120 Energy is a standard tick.
        int RefreshRate { get; }

        // The Action to be scheduled.
        ICommand Act();
    }
}
