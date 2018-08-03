using Pcg;
using Roguelike.Core;
using Roguelike.Utils;
using System;
using System.Collections.Generic;

namespace Roguelike.World
{
    class JaggedMapGenerator : MapGenerator
    {
        private const int _ROOM_SIZE = 5;
        private const int _ROOM_VARIANCE = 2;

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
            IList<(int x, int y, WallFacing dir)> openPoints = new List<(int x, int y, WallFacing dir)>();
            openPoints = AddOpenPoints(first, openPoints);

            // Randomly choose an open point, place a room there, and update the list of points.
            for (int i = 0; i < 10; i++)
            {
                (int availX, int availY, WallFacing dir) = openPoints[Rand.Next(openPoints.Count)];

                if (dir != WallFacing.E)
                    continue;

                Room room;
                int width = (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE);
                int height = (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE);

                Console.WriteLine($"x: {availX}\ty: {availY}\t{width} x {height}");

                switch (dir)
                {
                    case WallFacing.E:
                        for (int j = height; j > 0; j--)
                        {
                            if (!PointOnMap(availX + 1, availY + j + 1) || !Map.Field[availX + 1, availY + j + 1].IsWall)
                            {
                                if (PointOnMap(availX + 1, availY - 2) && Map.Field[availX + 1, availY - 2].IsWall)
                                    availY--;
                                else
                                    height--;
                            }
                        }

                        room = new Room(availX + 1, availY, width, height);
                        for (int dy = -1; dy <= room.Height; dy++)
                        {
                            openPoints.Remove((room.Left - 1, room.Y + dy, dir));
                        }
                        break;
                    case WallFacing.S:
                        room = new Room(availX, availY + 1, width, height);
                        for (int dx = 0; dx < room.Width; dx++)
                        {
                            openPoints.Remove((room.X + dx, room.Top, dir));
                        }
                        break;
                    case WallFacing.W:
                        room = new Room(availX - width, availY - height, width, height);
                        for (int dy = 0; dy < room.Height; dy++)
                        {
                            openPoints.Remove((room.Right, room.Y + dy, dir));
                        }
                        break;
                    case WallFacing.N:
                        room = new Room(availX - width, availY - height, width, height);
                        for (int dx = 0; dx < room.Width; dx++)
                        {
                            openPoints.Remove((room.X + dx, room.Bottom, dir));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dir));
                }

                CreateRoom(room);
                //openPoints = AddOpenPoints(room, openPoints);
            }

            foreach (var vv in openPoints)
            {
                Map.Field[vv.x, vv.y].Type = Data.TerrainType.Ice;
            }

            PlaceActors();

            return Map;
        }

        private IList<(int x, int y, WallFacing dir)> AddOpenPoints(Room room,
            IList<(int x, int y, WallFacing dir)> openPoints)
        {
            for (int x = room.Left; x < room.Right; x++)
            {
                int yTop = room.Top - 1;
                int yBottom = room.Bottom;

                if (PointOnMap(x, yTop) && Map.Field[x, yTop].IsWall)
                    openPoints.Add((x, yTop, WallFacing.N));

                if (PointOnMap(x, yBottom) && Map.Field[x, yBottom].IsWall)
                    openPoints.Add((x, yBottom, WallFacing.S));
            }

            for (int y = room.Top; y < room.Bottom; y++)
            {
                int xLeft = room.Left - 1;
                int xRight = room.Right;

                if (PointOnMap(xLeft, y) && Map.Field[xLeft, y].IsWall)
                    openPoints.Add((xLeft, y, WallFacing.W));

                if (PointOnMap(xRight, y) && Map.Field[xRight, y].IsWall)
                    openPoints.Add((xRight, y, WallFacing.E));
            }

            return openPoints;
        }

        private enum WallFacing
        {
            N, E, S, W
        }
    }
}
