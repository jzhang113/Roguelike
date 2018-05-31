namespace Roguelike.Core
{
    struct Room
    {
        private int _width;
        private int _height;

        public int X { get; internal set; }
        public int Y { get; internal set; }
        public int Width { get => _width; private set => _width = (value >= 2) ? value : 2; }
        public int Height { get => _height; private set => _height = (value >= 2) ? value : 2; }

        public int Area { get => Width * Height; }
        public (int, int) Center { get => (X + Width / 2, Y + Height / 2); }
        public (int, int) TopLeft { get => (X, Y); }
        public (int, int) BottomRight { get => (X + Width, Y + Height); }

        public int Left { get => X; }
        public int Right { get => X + Width; }
        public int Top { get => Y; }
        public int Bottom { get => Y + Height; }

        public Room(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            _width = width;
            _height = height;
        }

        public Room(int width, int height)
        {
            X = 0;
            Y = 0;
            _width = width;
            _height = height;
        }

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
