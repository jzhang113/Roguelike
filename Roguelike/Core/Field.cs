using System.Collections;
using System.Collections.Generic;

namespace Roguelike.Core
{
    class Field : IEnumerable<Terrain>
    {
        private Terrain[][] _field;
        private int _width;
        private int _height;

        public Field(int width, int height)
        {
            _width = width;
            _height = height;

            _field = new Terrain[width][];
            for (int i = 0; i < width; i++)
            {
                _field[i] = new Terrain[height];

                for (int j = 0; j < height; j++)
                {
                    _field[i][j] = new Terrain(true, 0, i, j);
                }
            }
        }

        public Terrain this[int i, int j] { get => IsValid(i, j) ? _field[i][j] : null; }

        public bool IsValid(int i, int j) => i >= 0 && i < _width && j >= 0 && j < _height;

        public IEnumerator<Terrain> GetEnumerator()
        {
            foreach (Terrain[] row in _field)
            {
                foreach (Terrain tile in row)
                    yield return tile;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (Terrain[] row in _field)
            {
                foreach (Terrain tile in row)
                    yield return tile;
            }
        }
    }
}
