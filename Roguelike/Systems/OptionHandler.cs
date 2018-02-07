using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roguelike.Systems
{
    class OptionHandler
    {
        internal enum MessageLevel { None, Normal, Verbose}

        // How much text to show.
        internal MessageLevel Verbosity { get; set; }

        // Are we using a fixed seed.
        internal bool FixedSeed { get; set; }

        // The value of the starting seed, if we are using one.
        internal int Seed { get; set; }

        internal OptionHandler(string[] args)
        {
            Verbosity = MessageLevel.Normal;
            ParseOption(args);
        }

        // Parse command line arguments
        private void ParseOption(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-s":
                        FixedSeed = true;
                        Seed = int.Parse(args[++i]);
                        break;
                    case "-q":
                        Verbosity = MessageLevel.None;
                        break;
                    case "-v":
                        Verbosity = MessageLevel.Verbose;
                        break;
                    default: throw new Exception(string.Format("Option {0} not recognized", args[i]));
                }
            }
        }
    }
}
