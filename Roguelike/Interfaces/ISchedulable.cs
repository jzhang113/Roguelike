using System;

namespace Roguelike.Interfaces
{
    // Describes things that may be scheduled by the EventScheduler, such as Actors or Effects.
    public interface ISchedulable : IComparable<ISchedulable>
    {
        // How much energy the Actor has. Actors may act when energy >= 0.
        int Energy { get; set; }

        // How much energy the Actor regains per game turn.
        // Q: Does resting allow the actor to continue gaining energy?
        // A: Resting should issue a wait action. Looks odd for fast characters and can be dangerous for slow ones. Allow dynamic resting?
        int RefreshRate { get; set; }

        // The Action to be scheduled.
        IAction Act();
    }
}
