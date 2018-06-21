using RLNET;
using Roguelike.Core;

namespace Roguelike
{
    static class OverlayHandler
    {
        public static string DisplayText;

        public static void Draw(RLConsole console)
        {
            console.Print(1, 1, DisplayText, Colors.TextHeading);
        }
    }
}