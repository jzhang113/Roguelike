using MessagePack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Core
{
    [MessagePackObject]
    class Field : IEnumerable<Tile>
    {
        [Key(0)]
        private readonly int _width;
        [Key(1)]
        private readonly int _height;
        [Key(2)]
        private readonly Tile[][] _field;

        public Field(int width, int height)
        {
            _width = width;
            _height = height;

            _field = new Tile[width][];
            for (int i = 0; i < width; i++)
            {
                _field[i] = new Tile[height];

                for (int j = 0; j < height; j++)
                    _field[i][j] = new Tile(i, j, Data.TerrainType.Wall);
            }
        }

        public Tile this[int i, int j]
        {
            get
            {
                int tempX = i, tempY = j;
                if (i < 0)
                    tempX = 0;
                else if (i >= _width)
                    tempX = _width - 1;

                if (j < 0)
                    tempY = 0;
                else if (j >= _height)
                    tempY = _height - 1;

                return _field[tempX][tempY];
            }
        }

        public bool IsValid(int i, int j) => i >= 0 && i < _width && j >= 0 && j < _height;

        public IEnumerator<Tile> GetEnumerator()
        {
            return _field.SelectMany(row => row).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
