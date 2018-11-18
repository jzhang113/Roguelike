using BearLib;
using Roguelike.Core;
using System.Drawing;

namespace Roguelike.UI
{
    internal class QtePanel
    {
        private readonly int _width;
        private readonly int _target;
        private readonly int _left;
        private readonly int _right;

        public QtePanel(int width, int target, int left, int right)
        {
            System.Diagnostics.Debug.Assert(left >= 0);
            System.Diagnostics.Debug.Assert(right <= width);

            _width = width;
            _target = target;
            _left = left;
            _right = right;
        }

        // draw the moving bar at a point
        public void Draw(LayerInfo layer, float current)
        {
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo()
            {
                LeftChar = '│', // 186
                RightChar = '│'
            });

            int height = layer.Height / 2;
            int xOffset = (layer.Width - _width) / 2;

            // draw line
            Terminal.Color(Colors.HighlightColor);
            for (int dx = 0; dx < _width + _target; dx++)
            {
                layer.Put(xOffset + dx, height, '═'); // 205
            }

            // draw accept region
            Terminal.Color(Swatch.DbGrass);
            for (int dx = _left; dx < _right + _target; dx++)
            {
                layer.Put(xOffset + dx, height, '█'); // 219
            }

            // draw moving bar
            int pos = (int)current;
            int offset = (int)((current - pos) * Terminal.State(Terminal.TK_CELL_WIDTH));

            Terminal.Color(Swatch.DbBlood);
            Terminal.Layer(layer.Z + 1);
            Terminal.PutExt(layer.X + xOffset + pos, layer.Y + height, offset, 0, '█'); // 178
            Terminal.Layer(layer.Z);
        }

        // draw the entire bar with a background
        public void Draw(LayerInfo layer, Color color)
        {
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo()
            {
                LeftChar = '│', // 186
                RightChar = '│'
            });

            int height = layer.Height / 2;
            int xOffset = (layer.Width - _width) / 2;

            // draw line
            Terminal.Color(Colors.HighlightColor);
            for (int dx = 0; dx < _width + _target; dx++)
            {
                layer.Put(xOffset + dx, height, '═'); // 205
            }

            // draw colored background
            Terminal.Color(color);
            for (int dx = 0; dx < _width + _target; dx++)
            {
                layer.Put(xOffset + dx, height, '█'); // 219
            }
        }
    }
}
