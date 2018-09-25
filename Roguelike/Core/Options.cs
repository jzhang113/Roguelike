using Roguelike.Systems;

namespace Roguelike.Core
{
    public class Options
    {
        // How much text to show.
        internal MessageLevel Verbosity { get; set; }

        // Are we using a fixed seed?
        internal bool FixedSeed { get; set; }

        // The value of the starting seed, if we are using one.
        internal int Seed { get; set; }
    }
}