using Pcg;
using Roguelike.Core;
using Roguelike.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.World
{
    class JaggedMapGenerator : MapGenerator
    {
        private const int _ROOM_SIZE = 8;
        private const int _ROOM_VARIANCE = 3;

        public JaggedMapGenerator(int width, int height, IEnumerable<LevelId> exits, PcgRandom random)
            : base(width, height, exits, random)
        {
        }

        public override MapHandler CreateMap()
        {
            // Place an initial room somewhere on the map.
            Room first = new Room(
                Rand.Next(Width - _ROOM_SIZE), Rand.Next(Height - _ROOM_SIZE),
                (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE),
                (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE));
            CreateRoom(first);

            // Maintain a list of points bordering the current list of rooms so we can attach
            // more rooms. Also track of the facing of the wall the point comes from.
            IList<(int x, int y)> openPoints = new List<(int x, int y)>();

            // Also keep track of the rooms and where they are.
            IList<Room> roomList = new List<Room>();
            int[,] occupied = new int[Width, Height];
            int counter = 0;

            // Ensure that the first room is on the map.
            if (first.Right >= Width)
                first.X -= first.Right - Width + 1;

            if (first.Bottom >= Height)
                first.Y -= first.Bottom - Height + 1;

            if (first.Left < 0)
                first.X = 0;

            if (first.Top < 0)
                first.Y = 0;

            TrackRoom(first, roomList, ++counter, ref occupied);
            AddOpenPoints(first, openPoints, occupied);

            for (int i = 0; i < 1000; i++)
            {
                if (openPoints.Count <= 0)
                    break;

                (int availX, int availY) = openPoints[Rand.Next(openPoints.Count)];

                int width = (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE);
                int height = (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE);
                Room room = AdjustRoom(availX, availY, width, height, occupied);

                RemoveOpenPoints(room, openPoints);
                TrackRoom(room, roomList, ++counter, ref occupied);
                AddOpenPoints(room, openPoints, occupied);
            }

            // Use the largest areas as rooms and triangulate them to calculate hallways.
            var enumerable = roomList.OrderByDescending(r => r.Area).Take((int) (roomList.Count * 0.3));

            foreach (Room room in enumerable)
            {
                CreateRoom(room);
            }

            PlaceActors();

            return Map;
        }

        // Try to fit the largest room possible up to width x height around ( availX, availY).
        // Note that this still fails sometimes and gives a collision when the shape is concave.
        private Room AdjustRoom(int availX, int availY, int width, int height,
            int[,] occupied)
        {
            int left = availX;
            int right = availX;
            int top = availY;
            int bottom = availY;

            for (int dx = 1; dx < width; dx++)
            {
                if (PointOnMap(availX + dx, availY) && occupied[availX + dx, availY] == 0)
                    right = availX + dx;
                else
                    break;
            }

            for (int dx = 1; dx < width - right + availX; dx++)
            {
                if (PointOnMap(availX - dx, availY) && occupied[availX - dx, availY] == 0)
                    left = availX - dx;
                else
                    break;
            }

            for (int dy = 1; dy < height; dy++)
            {
                if (PointOnMap(availX, availY + dy) && occupied[availX, availY + dy] == 0)
                    bottom = availY + dy;
                else
                    break;
            }

            for (int dy = 1; dy < height - bottom + availY; dy++)
            {
                if (PointOnMap(availX, availY - dy) && occupied[availX, availY - dy] == 0)
                    top = availY - dy;
                else
                    break;
            }

            int newWidth = right - left;
            int newHeight = bottom  -top;
            return new Room(left, top, newWidth, newHeight);
        }

        private static void TrackRoom(Room room, IList<Room> roomList, int counter,
            ref int[,] occupied)
        {
            int area = 0;
            for (int x = room.Left; x <= room.Right; x++)
            {
                for (int y = room.Top; y <= room.Bottom; y++)
                {
                    if (occupied[x, y] != 0)
                        continue;

                    occupied[x, y] = counter;
                    area++;
                }
            }

            if (area > 0)
                roomList.Add(room);
        }

        private void AddOpenPoints(Room room,
            ICollection<(int x, int y)> openPoints, int[,] occupied)
        {
            for (int x = room.Left; x <= room.Right; x++)
            {
                int yTop = room.Top - 1;
                int yBottom = room.Bottom + 1;

                if (PointOnMap(x, yTop) && occupied[x, yTop] == 0)
                    openPoints.Add((x, yTop));

                if (PointOnMap(x, yBottom) && occupied[x, yBottom] == 0)
                    openPoints.Add((x, yBottom));
            }

            for (int y = room.Top; y <= room.Bottom; y++)
            {
                int xLeft = room.Left - 1;
                int xRight = room.Right + 1;

                if (PointOnMap(xLeft, y) && occupied[xLeft, y] == 0)
                    openPoints.Add((xLeft, y));

                if (PointOnMap(xRight, y) && occupied[xRight, y] == 0)
                    openPoints.Add((xRight, y));
            }
        }

        private static void RemoveOpenPoints(Room room,
            ICollection<(int x, int y)> openPoints)
        {
            // Only need to check the edges since the adjust step already fits the rectangles.
            for (int x = room.Left; x <= room.Right; x++)
            {
                openPoints.Remove((x, room.Top));
                openPoints.Remove((x, room.Bottom));
            }

            for (int y = room.Top; y <= room.Bottom; y++)
            {
                openPoints.Remove((room.Left, y));
                openPoints.Remove((room.Right, y));
            }
        }
    }
}
