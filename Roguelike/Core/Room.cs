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

        public bool Intersects(Room other)
        {
            return (other.X >= X && other.X < X + _width &&
                    other.Y >= Y && other.Y < Y + _height) ||
                   (other.X + other.Width >= X && other.X + other.Width < X + _width &&
                    other.Y + other.Height >= Y && other.Y + other.Height < Y + _height);
        }
    }
}
