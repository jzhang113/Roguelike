using RLNET;
using Roguelike.Core;

namespace Roguelike.Systems
{
    static class OverlayHandler
    {
        public static string DisplayText { get; set; }
        public static RLColor[,] Background { get; }

        static OverlayHandler()
        {
            Background = new RLColor[Game.Config.Map.Width, Game.Config.Map.Height];
        }

        public static void Draw(RLConsole console)
        {
            console.Print(1, 1, DisplayText, Colors.TextHeading);

            for (int i = 0; i < Game.Config.Map.Width; i++)
            {
                for (int j = 0; j < Game.Config.Map.Height; j++)
                {
                    console.SetBackColor(i - Camera.X, j - Camera.Y, Background[i, j]);
                }
            }
        }

        public static void ClearBackground()
        {
            for (int i = 0; i < Game.Config.Map.Width; i++)
            {
                for (int j = 0; j < Game.Config.Map.Height; j++)
                {
                    Background[i, j] = Colors.FloorBackground;
                }
            }
        }
    }
}