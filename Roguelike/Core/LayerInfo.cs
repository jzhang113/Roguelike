using BearLib;
using System.Drawing;

namespace Roguelike.Core
{
    public class LayerInfo
    {
        public string Name { get; }
        public int Z { get; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public LayerInfo(string name, int z, int x, int y, int width, int height)
        {
            Name = name;
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
                System.Diagnostics.Debug.WriteLine($"Warning: {x} {y} out of bounds on layer {Name}");
        }

        public void Print(Rectangle layout, string text, ContentAlignment alignment)
        {
            layout.Offset(X, Y);
            Terminal.Print(layout, alignment, text);
        }

        public void Print(int y, string text, ContentAlignment alignment = ContentAlignment.TopLeft)
        {
            if (y < Height)
                Terminal.Print(new Rectangle(X, Y + y, Width, 1), alignment, text);
            else
                System.Diagnostics.Debug.WriteLine($"Warning: line {y} out of bounds on layer {Name}");
        }

        public void Print(int x, int y, string text)
        {
            if (x + text.Length < Width && y < Height)
                Terminal.Print(X + x, Y + y, text);
            else
                System.Diagnostics.Debug.WriteLine($"Warning: line {y} out of bounds on layer {Name}");
        }

        public void Clear() => Terminal.ClearArea(X, Y, Width, Height);
    }
}
