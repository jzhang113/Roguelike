using Roguelike.Core;
using RogueSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Systems
{
    class MapGenerator
    {
        private static int _minRoomSize = 5;
        private static int _maxRoomSize = 15;

        private readonly int _width;
        private readonly int _height;
        private readonly DungeonMap _map;

        public MapGenerator(int width, int height)
        {
            _width = width;
            _height = height;
            _map = new DungeonMap();
        }

        public DungeonMap CreateMap()
        {
            _map.Initialize(_width, _height);
            IList<Rectangle> roomList = new List<Rectangle>();
            int totalArea = _width * _height;
            int area = 0;
            int attempts = 0;
            int x, y, width, height;
            Rectangle newRoom;
            bool intersect;

            while (area < 0.6 * totalArea && attempts++ < 1000)
            {
                width = Game.Random.Next(_minRoomSize, _maxRoomSize);
                height = Game.Random.Next(_minRoomSize, _maxRoomSize);
                x = Game.Random.Next(1, _width - width - 1);
                y = Game.Random.Next(1, _height - height - 1);

                newRoom = new Rectangle(x, y, width, height);
                intersect = roomList.Any(room => newRoom.Intersects(room));

                if (!intersect)
                {
                    roomList.Add(newRoom);
                    area += width * height;
                }
            }

            IList<Rectangle> roomListCopy = new List<Rectangle>(roomList.Count);

            foreach (Rectangle room in roomList)
            {
                CreateRoom(room);

                roomListCopy.Add(new Rectangle(room.X, room.Y, room.Width, room.Height));
            }
          
            while (roomList.Count > 0)
            {
                int index = Game.Random.Next(roomList.Count - 1);
                Rectangle a = roomList[index];
                roomList.RemoveAt(index);

                Rectangle b = roomListCopy[Game.Random.Next(roomListCopy.Count - 1)];

                int x1 = Game.Random.Next(a.Width) + a.X;
                int y1 = Game.Random.Next(a.Height) + a.Y;

                int x2 = Game.Random.Next(b.Width) + b.X;
                int y2 = Game.Random.Next(b.Height) + b.Y;

                int dx = Math.Abs(x1 - x2);
                int dy = Math.Abs(y1 - y2);

                if (x1 < x2)
                    CreateRoom(new Rectangle(x1, y1, dx, 1));
                else
                    CreateRoom(new Rectangle(x2, y2, dx, 1));

                if (y1 < y2)
                    CreateRoom(new Rectangle(x1, y1, 1, dy));
                else
                    CreateRoom(new Rectangle(x2, y2, 1, dy));
            }

            return _map;
        }

        private int Distance(Point a, Point b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        private void CreateRoom(Rectangle rect)
        {
            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, true);
                }
            }
        }
    }
}
