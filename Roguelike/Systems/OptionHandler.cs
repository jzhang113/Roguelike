using Roguelike.Core;
using System;

namespace Roguelike.Systems
{
    static class OptionHandler
    {
        // Parse command line arguments
        public static Options ParseOptions(string[] args)
        {
            Options options = new Options
            {
                Verbosity = MessageLevel.Normal
            };

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-s":
                        options.FixedSeed = true;
                        options.Seed = int.Parse(args[++i]);
                        break;
                    case "-q":
                        options.Verbosity = MessageLevel.Minimal;
                        break;
                    case "-v":
                        options.Verbosity = MessageLevel.Verbose;
                        break;
                    default: throw new Exception($"Option {args[i]} not recognized");
                }
            }

            return options;
        }
    }
}
