using RLNET;

namespace Roguelike.Core
{
    class ConsoleInfo
    {
        public RLConsole Console { get; }
        public int X { get; }
        public int Y { get; }

        public ConsoleInfo(RLConsole console, int x, int y)
        {
            Console = console;
            X = x;
            Y = y;
        }
    }
}
