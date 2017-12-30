using Roguelike.Interfaces;

namespace Roguelike.Core
{
    class Move
    {
        public static ICommand N { get; } = new MoveCommand(0, -1);
        public static ICommand NE { get; } = new MoveCommand(1, -1);
        public static ICommand E { get; } = new MoveCommand(1, 0);
        public static ICommand SE { get; } = new MoveCommand(1, 1);
        public static ICommand S { get; } = new MoveCommand(0, 1);
        public static ICommand SW { get; } = new MoveCommand(-1, 1);
        public static ICommand W { get; } = new MoveCommand(-1, 0);
        public static ICommand NW { get; } = new MoveCommand(-1, -1);
        
    }
}
