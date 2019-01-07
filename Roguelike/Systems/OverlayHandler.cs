using BearLib;
using Roguelike.Core;
using System.Drawing;

namespace Roguelike.Systems
{
    public class OverlayHandler
    {
        public string DisplayText { private get; set; }

        private Color[,] TileColor { get; }
        private bool[,] IsSet { get; }

        private readonly int _width;
        private readonly int _height;

        public OverlayHandler(int width, int height)
        {
            _width = width;
            _height = height;

            TileColor = new Color[_width, _height];
            IsSet = new bool[_width, _height];
        }

        public void Set(int x, int y, Color color)
        {
            if (x >= _width || y >= _height || x < 0 || y < 0)
                return;

            TileColor[x, y] = color;
            IsSet[x, y] = true;
        }

        public void Unset(int x, int y)
        {
            if (x >= _width || y >= _height || x < 0 || y < 0)
                return;

            IsSet[x, y] = false;
        }

        public void Clear()
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    IsSet[i, j] = false;
                }
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
                    int viewX = i + Camera.X;
                    int viewY = j + Camera.Y;

                    if (IsSet[viewX, viewY])
                    {
                        Terminal.Color(TileColor[viewX, viewY]);
                        layer.Put(i, j, '█');
                    }
                }
            }

            Terminal.Composition(false);
        }
    }
}