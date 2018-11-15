using Pcg;
using Roguelike.Core;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using TriangleNet.Meshing;

namespace Roguelike.World
{
    internal class JaggedMapGenerator : MapGenerator
    {
        private const int _ROOM_SIZE = 3;
        private const int _ROOM_VARIANCE = 2;

        private const double _WIDTH_SIZE_MULT = 1;
        private const double _WIDTH_VAR_MULT = 1;
        private const double _HEIGHT_SIZE_MULT = 1;
        private const double _HEIGHT_VAR_MULT = 1;

        private const double _FILL_PERCENT = 0.05;
        private const double _LOOP_CHANCE = 0.15;

        public JaggedMapGenerator(int width, int height, IEnumerable<LevelId> exits, PcgRandom random)
            : base(width, height, exits, random)
        {
        }

        protected override void CreateMap()
        {
            // Maintain a list of points bordering the current list of rooms so we can attach
            // more rooms. Also track of the facing of the wall the point comes from.
            IList<(int x, int y)> openPoints = new List<(int x, int y)>();

            // Also keep track of the rooms and where they are.
            IList<Room> roomList = new List<Room>();
            int[,] occupied = new int[Width, Height];

            // Set up the initial placement of the map. Include special features, if any.
            int counter = InitialPlacement(roomList, openPoints, ref occupied);

            while (openPoints.Count > 0)
            {
                // Choose a random point to create a room around;
                (int availX, int availY) = openPoints[Rand.Next(openPoints.Count)];

                // Fit a room around the point as best as possible. AdjustRoom should avoid most
                // collisions between rooms.
                int width = (int)Rand.NextNormal(
                    _WIDTH_SIZE_MULT * _ROOM_SIZE,
                    _WIDTH_VAR_MULT * _ROOM_VARIANCE);
                int height = (int)Rand.NextNormal(
                    _HEIGHT_SIZE_MULT * _ROOM_SIZE,
                    _HEIGHT_VAR_MULT * _ROOM_VARIANCE);
                Room room = AdjustRoom(availX, availY, width, height, occupied);

                // Update the room list, the open point list, and the location grid.
                RemoveOpenPoints(room, openPoints);
                if (TrackRoom(room, roomList, counter + 1, ref occupied))
                    counter++;
                AddOpenPoints(room, openPoints, occupied);
            }

            // Use the largest areas as rooms and triangulate them to calculate hallways.
            RoomList = roomList
                .OrderByDescending(r => r.Area)
                .Take((int)(roomList.Count * _FILL_PERCENT))
                .ToList();

            // If we don't get enough rooms, we can't (and don't need to) triangulate, so just
            // draw in what we have.
            if (RoomList.Count == 1)
            {
                CreateRoomWithoutBorder(RoomList[0]);
            }
            else if (RoomList.Count == 2)
            {
                // Add the only hallway to the hall list
                Adjacency = new ICollection<int>[] { new[] { 1 }, new[] { 0 } };

                ClearRoomsBetween(RoomList[0], RoomList[1], roomList, occupied);
            }
            else
            {
                Polygon polygon = new Polygon();
                foreach (Room room in RoomList)
                {
                    polygon.Add(new Vertex(room.Center.X, room.Center.Y));
                }
                IMesh delaunay = polygon.Triangulate();

                // Reduce the number of edges and clear out rooms along the remaining edges.
                var edges = TrimEdges(delaunay.Edges, RoomList).ToList();
                Adjacency = BuildAdjacencyList(edges, RoomList.Count);

                foreach (Edge edge in edges)
                {
                    ClearRoomsBetween(RoomList[edge.P0], RoomList[edge.P1], roomList, occupied);
                }
            }

            PostProcess();
            AsciiPrint();
        }

        // Set up any special features on the level. After setup is complete, return the value the
        // room ID counter should start at.
        private int InitialPlacement(ICollection<Room> roomList,
            ICollection<(int x, int y)> openPoints,
            ref int[,] occupied)
        {
            // TODO: load starting layouts from json / xp
            Room first = new Room(
                Rand.Next(Width - _ROOM_SIZE), Rand.Next(Height - _ROOM_SIZE),
                (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE),
                (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE));

            // Ensure that the first room is on the map.
            if (first.Right >= Width)
                first.X -= first.Right - Width + 1;

            if (first.Bottom >= Height)
                first.Y -= first.Bottom - Height + 1;

            if (first.Left < 0)
                first.X = 0;

            if (first.Top < 0)
                first.Y = 0;

            if (TrackRoom(first, roomList, 1, ref occupied))
            {
                AddOpenPoints(first, openPoints, occupied);
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void ClearRoomsBetween(Room r1, Room r2, IList<Room> roomList, int[,] occupied)
        {
            int x0 = Rand.Next(r1.Left + 1, r1.Right);
            int y0 = Rand.Next(r1.Top + 1, r1.Bottom);
            int x1 = Rand.Next(r2.Left + 1, r2.Right);
            int y1 = Rand.Next(r2.Top + 1, r2.Bottom);
            IEnumerable<Tile> path = Map.GetStraightLinePath(x0, y0, x1, y1);

            foreach (Tile tile in path)
            {
                if (!tile.IsWall)
                    continue;

                // Zero corresponds to an unfilled tile, so we need to reduce the ID by 1.
                int roomID = occupied[tile.X, tile.Y] - 1;
                if (roomID >= 0)
                {
                    CreateRoomWithoutBorder(roomList[roomID]);
                }
                else
                {
                    // Map may not be fully tiled, so clear out any untouched squares to avoid
                    // disconnected regions.
                    tile.Type = Data.TerrainType.Stone;
                }
            }
        }

        // Clean up the map by removing stray walls.
        // TODO: fill in 1-tile holes without cutting the map
        // TODO: identify and mark islands
        // TODO: identify and mark peninsulas
        private void PostProcess()
        {
            // Sweep from top to bottom.
            for (int x = 1; x < Width - 1; x++)
            {
                // Keep a running count of consecutive walls
                // Start the wall count at an arbitrarily high number so walls near edges don't get
                // removed unnecessarily.
                int wallCount = 10;
                for (int y = 3; y < Height - 3; y++)
                {
                    if (Map.Field[x, y].IsWall)
                    {
                        wallCount++;
                    }
                    else
                    {
                        if (wallCount == 1)
                            Map.Field[x, y - 1].Type = Data.TerrainType.Stone;

                        wallCount = 0;
                    }
                }
            }

            // Sweep from left to right.
            for (int y = 1; y < Height - 1; y++)
            {
                int wallCount = 10;
                for (int x = 3; x < Width - 3; x++)
                {
                    if (Map.Field[x, y].IsWall)
                    {
                        wallCount++;
                    }
                    else
                    {
                        if (wallCount == 1)
                            Map.Field[x - 1, y].Type = Data.TerrainType.Stone;

                        wallCount = 0;
                    }
                }
            }
        }

        // Try to fit the largest room possible up to width x height around (availX, availY).
        // TODO: rewrite AdjustRoom to grow the room with a radial sweepline to avoid collision
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
            int newHeight = bottom - top;
            return new Room(left, top, newWidth, newHeight);
        }

        // Add a non-zero size room to the room list and update the occupied matrix. Returns false
        // if a room was not added.
        private static bool TrackRoom(Room room, ICollection<Room> roomList, int counter,
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
            {
                roomList.Add(room);
                return true;
            }
            else
            {
                return false;
            }
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

        // Use Prim's algorithm to generate a MST of edges.
        private IEnumerable<Edge> TrimEdges(IEnumerable<Edge> edges,
            IList<Room> rooms)
        {
            List<Edge> allEdges = edges.ToList();
            ICollection<int>[] adjacency = BuildAdjacencyList(allEdges, rooms.Count);
            // Comparator for MapVertex is defined to give negated
            MaxHeap<MapVertex> pq = new MaxHeap<MapVertex>(rooms.Count);

            var (firstX, firstY) = rooms[0].Center;
            pq.Add(new MapVertex(0, firstX, firstY, 0));

            bool[] inMst = new bool[rooms.Count];
            double[] weight = new double[rooms.Count];
            int[] parent = new int[rooms.Count];

            for (int i = 0; i < rooms.Count; i++)
            {
                weight[i] = double.MaxValue;
                parent[i] = -1;
            }

            while (pq.Count > 0)
            {
                MapVertex min = pq.PopMax();
                inMst[min.ID] = true;

                foreach (int neighborID in adjacency[min.ID])
                {
                    if (inMst[neighborID])
                        continue;

                    var (neighborX, neighborY) = rooms[neighborID].Center;
                    double newWeight = Distance.EuclideanDistanceSquared(min.X, min.Y,
                        neighborX, neighborY);

                    if (weight[neighborID] > newWeight)
                    {
                        weight[neighborID] = newWeight;
                        pq.Add(new MapVertex(neighborID, neighborX, neighborY, newWeight));
                        parent[neighborID] = min.ID;
                    }
                }
            }

            ICollection<Edge> graph = new HashSet<Edge>();
            for (int i = 0; i < rooms.Count; i++)
            {
                if (parent[i] != -1)
                    graph.Add(new Edge(i, parent[i]));
            }

            // Add back some edges so that there are some loops.
            // TODO: smarter checking to add edges between the farthest rooms.
            for (int i = 0; i < allEdges.Count * _LOOP_CHANCE; i++)
            {
                graph.Add(allEdges[Rand.Next(allEdges.Count)]);
            }

            return graph;
        }

        private static ICollection<int>[] BuildAdjacencyList(IEnumerable<Edge> edges, int size)
        {
            ICollection<int>[] adjacency = new ICollection<int>[size];
            for (int i = 0; i < size; i++)
            {
                adjacency[i] = new List<int>();
            }

            foreach (Edge edge in edges)
            {
                adjacency[edge.P0].Add(edge.P1);
                adjacency[edge.P1].Add(edge.P0);
            }

            return adjacency;
        }

        private void AsciiPrint()
        {
            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    Console.Write(Map.Field[i, j].IsWall ? '#' : '.');
                }
                Console.WriteLine();
            }
        }

        private readonly struct MapVertex : IComparable<MapVertex>
        {
            public int ID { get; }
            public int X { get; }
            public int Y { get; }
            private double Weight { get; }

            public MapVertex(int id, int x, int y, double weight)
            {
                ID = id;
                X = x;
                Y = y;
                Weight = weight;
            }

            public int CompareTo(MapVertex other)
            {
                return (int)(other.Weight - Weight);
            }
        }
    }
}
