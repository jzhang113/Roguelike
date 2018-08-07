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
    class JaggedMapGenerator : MapGenerator
    {
        private const int _ROOM_SIZE = 5;
        private const int _ROOM_VARIANCE = 2;
        private const int _ROOM_ATTEMPTS = 1000;
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

            for (int i = 0; i < _ROOM_ATTEMPTS; i++)
            {
                // Nowhere else to put rooms, so we are done.
                if (openPoints.Count <= 0)
                    break;

                // Choose a random point to create a room around;
                (int availX, int availY) = openPoints[Rand.Next(openPoints.Count)];

                // Fit a room around the point as best as possible. AdjustRoom should avoid most
                // collisions between rooms.
                int width = (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE);
                int height = (int)Rand.NextNormal(_ROOM_SIZE, _ROOM_VARIANCE);
                Room room = AdjustRoom(availX, availY, width, height, occupied);

                // Update the room list, the open point list, and the location grid.
                RemoveOpenPoints(room, openPoints);
                if (TrackRoom(room, roomList, counter + 1, ref occupied))
                    counter++;
                AddOpenPoints(room, openPoints, occupied);
            }

            // Use the largest areas as rooms and triangulate them to calculate hallways.
            var roomCenters = roomList
                .OrderByDescending(r => r.Area)
                .Take((int)(roomList.Count * _FILL_PERCENT))
                .Select(r => new Vertex(r.Center.X, r.Center.Y));

            Polygon polygon = new Polygon();
            foreach (Vertex vertex in roomCenters)
            {
                polygon.Add(vertex);
            }
            IMesh delaunay = polygon.Triangulate();

            IList<(int X, int Y)> vertices = new (int X, int Y)[delaunay.Vertices.Count];
            foreach (Vertex vertex in delaunay.Vertices)
            {
                vertices[vertex.ID] = ((int)vertex.X, (int)vertex.Y);
            }

            // Reduce the number of edges and clear out rooms along the remaining edges.
            foreach (Edge edge in TrimEdges(delaunay.Edges, vertices))
            {
                (int x0, int y0) = vertices[edge.P0];
                (int x1, int y1) = vertices[edge.P1];
                IEnumerable<Tile> path = Map.GetStraightLinePath(x0, y0, x1, y1);

                foreach (Tile tile in path)
                {
                    if (!tile.IsWall)
                        continue;

                    // Zero corresponds to an unfilled tile, so we need to reduce the ID by 1.
                    int roomID = occupied[tile.X, tile.Y] - 1;
                    if (roomID >= 0)
                        CreateRoomWithoutBorder(roomList[roomID]);
                    else
                        // Room may not be fully tiled, so clear out any untouched squares to avoid
                        // disconnected regions.
                        tile.Type = Data.TerrainType.Stone;
                }
            }
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
                return 0;
            else
            {
                AddOpenPoints(first, openPoints, occupied);
                return 1;
            }
        }

        // Try to fit the largest room possible up to width x height around ( availX, availY).
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
            IList<(int X, int Y)> vertices)
        {
            List<Edge> allEdges = edges.ToList();
            ICollection<int>[] adjacency = BuildAdjacencyList(allEdges, vertices.Count);
            // Comparator for MapVertex is defined to give negated
            MaxHeap<MapVertex> pq = new MaxHeap<MapVertex>(vertices.Count);
            
            var (firstX, firstY) = vertices[0];
            pq.Add(new MapVertex(0, firstX, firstY, 0));

            bool[] inMst = new bool[vertices.Count];
            double[] weight = new double[vertices.Count];
            int[] parent = new int[vertices.Count];
            
            for (int i = 0; i < vertices.Count; i++)
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

                    var (neighborX, neighborY) = vertices[neighborID];
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
            for (int i = 0; i < vertices.Count; i++)
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

        private struct MapVertex : IComparable<MapVertex>
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
