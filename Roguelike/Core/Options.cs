namespace Roguelike.Core
{
    internal class Options
    {
        // How much text to show.
        internal Enums.MessageLevel Verbosity { get; set; }

        // Are we using a fixed seed?
        internal bool FixedSeed { get; set; }

        // The value of the starting seed, if we are using one.
        internal int Seed { get; set; }
    }
}