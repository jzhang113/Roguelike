namespace Roguelike.Core
{
    class Field
    {
        private Terrain[,] _field;
        private int _width;
        private int _height;

        public Field(int width, int height)
        {
            _width = width;
            _height = height;

            _field = new Terrain[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    _field[i, j] = new Terrain(true, 0, i, j);
                }
            }
        }

        public Terrain this[int i, int j] { get => IsValid(i, j) ? _field[i,j] : null; }

        public bool IsValid(int i, int j) => i >= 0 && i < _width && j >= 0 && j < _height;
    }
}
