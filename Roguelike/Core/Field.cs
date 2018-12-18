using System;
using System.Collections;
using System.Collections.Generic;

namespace Roguelike.Core
{
    [Serializable]
    internal class Field : IEnumerable<Tile>
    {
        private readonly Tile[] _field;
        private readonly int _width;
        private readonly int _height;

        public Field(int width, int height)
        {
            _width = width;
            _height = height;

            _field = new Tile[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                    _field[y * height + x] = new Tile(x, y, Data.TerrainType.Wall);
            }
        }

        public Tile this[int i, int j] => _field[i + _width * j];

        public bool IsValid(int i, int j) => i >= 0 && i < _width && j >= 0 && j < _height;

        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (Tile tile in _field)
                yield return tile;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
