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

        private readonly int _viewWidth;
        private readonly int _viewHeight;

        public OverlayHandler(int width, int height)
        {
            _viewWidth = width + 1;
            _viewHeight = height + 1;

            Foreground = new Color[_viewWidth, _viewHeight];
            Background = new Color[_viewWidth, _viewHeight];
            SetBackground = new bool[_viewWidth, _viewHeight];
            SetForeground = new bool[_viewWidth, _viewHeight];
        }

        public void Set(int x, int y, Color color, bool background = false)
        {
            int xPos = x - Camera.X;
            int yPos = y - Camera.Y;

            if (xPos >= _viewWidth || yPos >= _viewHeight
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

            for (int i = 0; i <= layer.Width; i++)
            {
                for (int j = 0; j <= layer.Height; j++)
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

            Terminal.Composition(false);
        }

        public void ClearBackground()
        {
            for (int i = 0; i < _viewWidth; i++)
            {
                for (int j = 0; j < _viewHeight; j++)
                {
                    SetBackground[i, j] = false;
                }
            }
        }

        public void ClearForeground()
        {
            for (int i = 0; i < _viewWidth; i++)
            {
                for (int j = 0; j < _viewHeight; j++)
                {
                    SetForeground[i, j] = false;
                }
            }
        }
    }
}