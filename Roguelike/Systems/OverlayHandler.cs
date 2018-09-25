using BearLib;
using Roguelike.Core;
using System.Drawing;

namespace Roguelike.Systems
{
    public class OverlayHandler
    {
        public string DisplayText { private get; set; }

        private Color[,] Background { get; }
        private Color[,] Foreground { get; }
        private bool[,] SetBackground { get; }
        private bool[,] SetForeground { get; }

        public OverlayHandler(int width, int height)
        {
            int viewWidth = width + 1;
            int viewHeight = height + 1;

            Background = new Color[viewWidth, viewHeight];
            Foreground = new Color[viewWidth, viewHeight];
            SetBackground = new bool[viewWidth, viewHeight];
            SetForeground = new bool[viewWidth, viewHeight];
        }

        public void Set(int x, int y, Color color, bool background = false)
        {
            int xPos = x - Camera.X;
            int yPos = y - Camera.Y;

            if (xPos > Game.Config.MapView.Width || yPos > Game.Config.MapView.Height
                || xPos < 0 || yPos < 0)
            {
                return;
            }

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

        public void Draw()
        {
            Terminal.Color(Colors.Text);
            Terminal.Print(1, 1, DisplayText);

            for (int i = 0; i <= Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j <= Game.Config.MapView.Height; j++)
                {
                    // TODO: can only set backgrounds on 0th layer
                    if (SetBackground[i, j])
                        Terminal.BkColor(Background[i, j]);

                    if (SetForeground[i, j])
                        Terminal.Color(Foreground[i, j]);

                    Terminal.Put(i, j, Terminal.Pick(i, j));
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