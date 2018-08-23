using System;

namespace Roguelike.Core
{
    static class Camera
    {
        public static int X { get; private set; }
        public static int Y { get; private set; }

        internal static void UpdateCamera()
        {
            int screenWidth = Game.Config.MapView.Width;
            int screenHeight = Game.Config.MapView.Height;

            // set left and top limits for the camera
            int startX = Math.Max(Game.Player.X - screenWidth / 2, 0);
            int startY = Math.Max(Game.Player.Y - screenHeight / 2, 0);

            // set right and bottom limits for the camera
            int xDiff = Game.Config.Map.Width - screenWidth;
            int yDiff = Game.Config.Map.Height - screenHeight;
            X = xDiff < 0 ? 0 : Math.Min(xDiff, startX);
            Y = yDiff < 0 ? 0 : Math.Min(yDiff, startY);
        }
    }
}