using RLNET;

namespace Roguelike.Systems
{
    static class MouseHandler
    {
        public static bool GetHoverPosition(RLMouse mouse, out (int X, int Y) pos)
        {
            int mapTop = Game.Config.MessageView.Height;
            int mapBottom = Game.Config.MessageView.Height + Game.Config.MapView.Height;
            int mapLeft = 0;
            int mapRight = Game.Config.MapView.Width;

            if (mouse.X > mapLeft && mouse.X < mapRight - 1 && mouse.Y > mapTop && mouse.Y < mapBottom - 1)
            {
                int xPos = mouse.X - mapLeft;
                int yPos = mouse.Y - mapTop;
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
