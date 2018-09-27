using BearLib;
using System.Drawing;

namespace Roguelike.Core
{
    public class LayerInfo
    {
        public int Z { get; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public LayerInfo(int z, int x, int y, int width, int height)
        {
            Z = z;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void Put(int x, int y, int code)
        {
            if (x <= Width && y <= Height)
                Terminal.Put(X + x, Y + y, code);
            else
                System.Diagnostics.Debug.WriteLine($"Warning: {x} {y} out of bounds on layer {Z}");
        }

        public void Print(int y, string text, ContentAlignment alignment = ContentAlignment.TopLeft)
        {
            if (y < Height)
                Terminal.Print(new Rectangle(X, Y + y, Width, 1), alignment, text);
            else
                System.Diagnostics.Debug.WriteLine($"Warning: line {y} out of bounds on layer {Z}");
        }

        public void Print(int x, int y, string text)
        {
            if (x + text.Length < Width && y < Height)
                Terminal.Print(X + x, Y + y, text);
            else
                System.Diagnostics.Debug.WriteLine($"Warning: line {y} out of bounds on layer {Z}");
        }

        public void Clear() => Terminal.ClearArea(X, Y, Width, Height);
    }
}
