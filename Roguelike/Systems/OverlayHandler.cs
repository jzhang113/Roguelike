using RLNET;
using Roguelike.Core;

namespace Roguelike.Systems
{
    class OverlayHandler
    {
        public string DisplayText { private get; set; }

        private RLColor[,] Background { get; }
        private RLColor[,] Foreground { get; }
        private bool[,] SetBackground { get; }
        private bool[,] SetForeground { get; }

        public OverlayHandler(int width, int height)
        {
            int viewWidth = width + 1;
            int viewHeight = height + 1;

            Background = new RLColor[viewWidth, viewHeight];
            Foreground = new RLColor[viewWidth, viewHeight];
            SetBackground = new bool[viewWidth, viewHeight];
            SetForeground = new bool[viewWidth, viewHeight];
        }

        public void Set(int x, int y, RLColor color, bool background = false)
        {
            int xPos = x - Camera.X;
            int yPos = y - Camera.Y;

            if (xPos > Game.Config.MapView.Width || yPos > Game.Config.MapView.Height
                || xPos < 0 || yPos < 0)
                return;

            if (background)
            {
                Background[xPos, yPos] = color;
                SetBackground[xPos, yPos] = true;
            }
            else
            {
                Foreground[xPos, yPos] = color;
                SetForeground[xPos, yPos] = true;
            }
        }

        public void Draw(RLConsole console)
        {
            console.Print(1, 1, DisplayText, Colors.TextHeading);

            for (int i = 0; i <= Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j <= Game.Config.MapView.Height; j++)
                {
                    if (SetBackground[i, j])
                        console.SetBackColor(i, j, Background[i, j]);

                    if (SetForeground[i, j])
                        console.SetBackColor(i, j, Foreground[i, j]);
                }
            }
        }

        public void ClearBackground()
        {
            for (int i = 0; i <= Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j <= Game.Config.MapView.Height; j++)
                {
                    SetBackground[i, j] = false;
                }
            }
        }

        public void ClearForeground()
        {
            for (int i = 0; i <= Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j <= Game.Config.MapView.Height; j++)
                {
                    SetForeground[i, j] = false;
                }
            }
        }
    }
}