namespace Roguelike.Core
{
    struct Room
    {
        private int _width;
        private int _height;

        public int X { get; internal set; }
        public int Y { get; internal set; }
        public int Width { get => _width; internal set => _width = (value >= 2) ? value : 2; }
        public int Height { get => _height; internal set => _height = (value >= 2) ? value : 2; }

        public int Area { get => _width * _height; }
        public (int, int) Center { get => (X + _width / 2, Y + _height / 2); }
        public (int, int) TopLeft { get => (X, Y); }
        public (int, int) BottomRight { get => (X + _width, Y + _height); }

        public bool Intersects(Room other)
        {
            return other.X < X + _width &&
                   X < other.X + other.Width &&
                   other.Y < Y + _height &&
                   Y < other.Y + other.Height;
        }

        public void Offset(int x, int y)
        {
            X += x;
            Y += y;
        }
    }
}
