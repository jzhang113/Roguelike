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

        public OverlayHandler(int viewWidth, int viewHeight)
        {
            Background = new RLColor[viewWidth, viewHeight];
            Foreground = new RLColor[viewWidth, viewHeight];
            SetBackground = new bool[viewWidth, viewHeight];
            SetForeground = new bool[viewWidth, viewHeight];
        }

        public void Set(int x, int y, RLColor color, bool background = false)
        {
            int xPos = x - Camera.X;
            int yPos = y - Camera.Y;

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

            for (int i = 0; i < Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j < Game.Config.MapView.Height; j++)
                {
                    if (SetBackground[i, j])
                        console.SetBackColor(i + Camera.X, j + Camera.Y, Background[i, j]);

                    if (SetForeground[i, j])
                        console.SetBackColor(i + Camera.X, j + Camera.Y, Foreground[i, j]);
                }
            }
        }

        public void ClearBackground()
        {
            for (int i = 0; i < Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j < Game.Config.MapView.Height; j++)
                {
                    SetBackground[i, j] = false;
                }
            }
        }

        public void ClearForeground()
        {
            for (int i = 0; i < Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j < Game.Config.MapView.Height; j++)
                {
                    SetForeground[i, j] = false;
                }
            }
        }
    }
}