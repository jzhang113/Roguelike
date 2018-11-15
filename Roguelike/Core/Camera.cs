using System;

namespace Roguelike.Core
{
    internal static class Camera
    {
        public static int X { get; private set; }
        public static int Y { get; private set; }

        internal static void UpdateCamera()
        {
            const int screenWidth = Data.Constants.MAPVIEW_WIDTH;
            const int screenHeight = Data.Constants.MAPVIEW_HEIGHT;

            // set left and top limits for the camera
            int startX = Math.Max(Game.Player.X - screenWidth / 2, 0);
            int startY = Math.Max(Game.Player.Y - screenHeight / 2, 0);

            // set right and bottom limits for the camera
            const int xDiff = Data.Constants.MAP_WIDTH - screenWidth;
            const int yDiff = Data.Constants.MAP_HEIGHT - screenHeight;
            X = xDiff < 0 ? 0 : Math.Min(xDiff, startX);
            Y = yDiff < 0 ? 0 : Math.Min(yDiff, startY);
        }
    }
}