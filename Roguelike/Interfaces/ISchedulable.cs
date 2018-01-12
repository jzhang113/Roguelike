using System;

namespace Roguelike.Interfaces
{
    // Describes things that may be added to the EventScheduler, such as Actors or Effects.
    interface ISchedulable : IComparable<ISchedulable>
    {
        // How much energy the actor has. Actors may act when energy >= 0.
        int Energy { get; set; }

        // How much energy the actor regains per game turn.
        // Q: Does resting allow the actor to continue gaining energy?
        // A: Resting should issue a wait action. Looks odd for fast characters and can be dangerous for slow ones. Allow dynamic resting?
        int RefreshRate { get; set; }

        IAction Act();
    }
}
