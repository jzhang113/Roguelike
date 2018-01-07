using Roguelike.Core;
using RogueSharp;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Systems
{
    class MapGenerator
    {
        private readonly int _minRoomSize = 5;
        private readonly int _maxRoomSize = 15;

        private readonly int _width;
        private readonly int _height;
        private readonly DungeonMap _map;

        public MapGenerator(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new DungeonMap(_width, _height);
        }

        public DungeonMap CreateMap(Random random)
        {
            _map.Initialize(_width, _height);
            IList<Rectangle> roomList = new List<Rectangle>();
            int totalArea = _width * _height;
            int area = 0;
            int attempts = 0;
            int x, y, width, height;
            Rectangle newRoom;
            bool intersect;

            while (area < 0.8 * totalArea && attempts++ < 1000)
            {
                width = random.Next(_minRoomSize, _maxRoomSize);
                height = random.Next(_minRoomSize, _maxRoomSize);
                x = random.Next(1, _width - width - 1);
                y = random.Next(1, _height - height - 1);

                newRoom = new Rectangle(x, y, width, height);
                intersect = roomList.Any(room => newRoom.Intersects(room));

                if (!intersect)
                {
                    roomList.Add(newRoom);
                    area += width * height;
                }
            }

            foreach (Rectangle room in roomList)
            {
                CreateRoom(room);
            }
            
            for (int i = _map.Width - 2; i > 0; i--)
            {
                for (int j = _map.Height - 2; j > 0; j--)
                {
                    if (!_map.GetCell(i, j).IsWalkable)
                    {
                        if (_map.GetCell(i - 1, j).IsWalkable)
                        {
                            if (_map.GetCell(i + 1, j).IsWalkable)
                            {
                                _map.SetCellProperties(i, j, true, true);
                                _map.Field[i, j].IsWalkable = true;
                            }
                        }

                        if (_map.GetCell(i, j - 1).IsWalkable)
                        {
                            if (_map.GetCell(i, j + 1).IsWalkable)
                            {
                                _map.SetCellProperties(i, j, true, true);
                                _map.Field[i, j].IsWalkable = true;
                            }
                        }
                    }
                }
            }

            return _map;
        }
        
        private void CreateRoom(Rectangle rect)
        {
            bool explored = true;

            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, explored);
                    _map.Field[x, y].IsWalkable = true;
                }
            }
        }

        private void CreateHallway(int x1, int y1, int x2, int y2)
        {
            int dx = Math.Abs(x1 - x2);
            int dy = Math.Abs(y1 - y2);

            if (x1 < x2)
            {
                if (y1 < y2)
                {
                    CreateRoom(new Rectangle(x2 - dx, y2, dx + 1, 1));
                    CreateRoom(new Rectangle(x1, y1, 1, dy + 1));
                }
                else
                {
                    CreateRoom(new Rectangle(x1, y1, dx + 1, 1));
                    CreateRoom(new Rectangle(x2, y2, 1, dy + 1));
                }
            }
            else
            {
                if (y1 < y2)
                {
                    CreateRoom(new Rectangle(x2, y2, dx + 1, 1));
                    CreateRoom(new Rectangle(x1, y1, 1, dy + 1));
                }
                else
                {
                    CreateRoom(new Rectangle(x1 - dx, y1, dx + 1, 1));
                    CreateRoom(new Rectangle(x2, y2, 1, dy + 1));
                }
            }
        }
    }
}
