namespace Roguelike.Utils
{
    internal readonly struct ColorInfo
    {
        public int X { get; }
        public int Y { get; }
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public double Brightness { get; }

        public ColorInfo(int x, int y, byte r, byte g, byte b, double bright)
        {
            X = x;
            Y = y;
            R = r;
            G = g;
            B = b;
            Brightness = bright;
        }
    }
}
