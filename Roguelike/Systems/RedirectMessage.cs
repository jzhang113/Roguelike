using Roguelike.Interfaces;

namespace Roguelike.Systems
{
    // Passes a message to notify if the current attempted Action will succeed or not. May provide
    // an alternative Action if the current Action will fail.
    public struct RedirectMessage
    {
        // Tells whether an Action succeeded or failed.
        public bool Success { get; }

        // The alternative Action to attempt if the current one failed.
        public ICommand Alternative { get; }

        public RedirectMessage(bool success)
        {
            Success = success;
            Alternative = null;
        }

        public RedirectMessage(bool success, ICommand alternative)
        {
            Success = success;
            Alternative = alternative;
        }
    }
}
