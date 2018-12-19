using BearLib;
using Roguelike.Core;
using Roguelike.Data;

namespace Roguelike.UI
{
    internal static class InfoPanel
    {
        public static void Draw(LayerInfo layer)
        {
            Game.ShowInfo = true;
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '╔', // 201
                BottomLeftChar = '╚', // 200
                TopChar = '═', // 205
                BottomChar = '═',
                LeftChar = '║' // 186
            });
            layer.Print(-1, $"{Constants.HEADER_LEFT}SCAN" +
                $"[color=white]{Constants.HEADER_SEP}DATA{Constants.HEADER_RIGHT}",
                System.Drawing.ContentAlignment.TopRight);

            // draw info
            Terminal.Color(Colors.Text);
        }
    }
}
