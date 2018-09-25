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
    }
}
