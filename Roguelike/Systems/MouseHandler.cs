using System;
using RLNET;

namespace Roguelike.Systems
{
    static class MouseHandler
    {
        public static bool GetHoverPosition(RLMouse mouse, out (int X, int Y) pos)
        {
            int screenWidth = Game.Config.MapView.Width;
            int screenHeight = Game.Config.MapView.Height;
            int halfWidth = screenWidth / 2;
            int halfHeight = screenHeight / 2;

            // set left and top limits for the camera
            int startX = Math.Max(Game.Player.X - halfWidth, 0);
            int startY = Math.Max(Game.Player.Y - halfHeight, 0);

            // set right and bottom limits for the camera
            startX = Math.Min(Game.Config.Map.Width - screenWidth, startX);
            startY = Math.Min(Game.Config.Map.Height - screenHeight, startY);

            int mapTop = Game.Config.MessageView.Height;
            int mapBottom = Game.Config.MessageView.Height + Game.Config.MapView.Height;
            int mapLeft = 0;
            int mapRight = Game.Config.MapView.Width;

            if (mouse.X > mapLeft && mouse.X < mapRight - 1 && mouse.Y > mapTop && mouse.Y < mapBottom - 1)
            {
                int xPos = mouse.X - mapLeft + startX;
                int yPos = mouse.Y - mapTop + startY;
                pos = (xPos, yPos);
                return true;
            }
            else
            {
                pos = (-1, -1);
                return false;
            }
        }

        public static bool GetClickPosition(RLMouse mouse, out (int X, int Y) pos)
        {
            if (mouse.GetLeftClick())
                return GetHoverPosition(mouse, out pos);

            pos = (-1, -1);
            return false;
        }
    }
}
