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

        public void Draw(LayerInfo layer)
        {
            Terminal.Color(Colors.Text);
            Terminal.Composition(true);
            layer.Print(1, DisplayText);

            for (int i = 0; i <= Game.Config.MapView.Width; i++)
            {
                for (int j = 0; j <= Game.Config.MapView.Height; j++)
                {
                    if (SetBackground[i, j])
                    {
                        Terminal.Color(Background[i, j]);
                        layer.Put(i, j, '█');
                    }

                    if (SetForeground[i, j])
                    {
                        Terminal.Color(Foreground[i, j]);
                        layer.Put(i, j, '█');
                    }
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