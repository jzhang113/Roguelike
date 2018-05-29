using Roguelike.Core;
using Roguelike.Utils;
using RogueSharp;
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
        private readonly Random _rand;
        private readonly MapHandler _map;

        public MapGenerator(int width, int height, Random rand)
        {
            _width = width;
            _height = height;
            _rand = rand;
            _map = new MapHandler(_width, _height);
        }

        public MapHandler FillMap()
        {
            IList<Room> roomsList = new List<Room>();
            int centerX = _width / 2;
            int centerY = _height / 2;

            for (int i = 0; i < 1000; i++)
            {
                Room room = new Room
                {
                    Width = (int)RandNormal(5, 1),
                    Height = (int)RandNormal(5, 1),
                    X = _rand.Next(centerX - 5, centerX + 5),
                    Y = _rand.Next(centerY - 5, centerY + 5)
                };

                bool first = true;
                bool horiz = false;
                bool intersects = false;
                int dx = 0, dy = 0;
                int iterations = 0;

                do
                {
                    intersects = false;
                    iterations++;

                    if (iterations == 100)
                    {
                        Console.WriteLine("Too many iterations:" + i);
                    }

                    foreach (Room prev in roomsList)
                    {
                        if (prev.Intersects(room))
                        {
                            intersects = true;
                            var (x1, y1) = prev.Center;
                            var (x2, y2) = room.Center;

                            if (first)
                            {
                                first = false;
                                dx = x2 - x1;
                                dy = y2 - y1;

                                if (dx == 0 && dy == 0)
                                {
                                    dx = (_rand.NextDouble() < 0.5) ? -1 : 1;
                                    dy = (_rand.NextDouble() < 0.5) ? -1 : 1;
                                }
                            }
                            else
                            {
                                if (dx > 0)
                                {
                                    dx += x2 - x1;
                                    if (horiz && dx < 1)
                                        dx = 1;
                                }
                                else
                                {
                                    dx += x2 - x1;
                                    if (horiz && dx > -1)
                                        dx = -1;
                                }

                                if (dy > 0)
                                {
                                    dy += y2 - y1;
                                    if (!horiz && dy < 1)
                                        dy = 1;
                                }
                                else
                                {
                                    dy += y2 - y1;
                                    if (!horiz && dy > -1)
                                        dy = -1;
                                }
                            }

                            int pushX = (dx > 0) ? prev.X + prev.Width - room.X : prev.X - room.X - room.Width;
                            int pushY = (dy > 0) ? prev.Y + prev.Height - room.Y : prev.Y - room.Y - room.Height;
                            double multiplier;

                            if (Math.Abs(dx) > Math.Abs(dy))
                            {
                                multiplier = (double)pushX / dx;
                                horiz = true;
                            }
                            else if (Math.Abs(dx) < Math.Abs(dy))
                            {
                                multiplier = (double)pushY / dy;
                                horiz = false;
                            }
                            else
                            {
                                if (Math.Abs(pushX) <= Math.Abs(pushY))
                                {
                                    multiplier = (double)pushX / dx;
                                    horiz = true;
                                }
                                else
                                {
                                    multiplier = (double)pushY / dy;
                                    horiz = false;
                                }
                            }

                            room.X = (int)(multiplier * dx) + room.X;
                            room.Y = (int)(multiplier * dy) + room.Y;
                        }
                    }
                } while (intersects && iterations < 100);

                roomsList.Add(room);
                CreateRoom2(room);
                //AsciiPrint(roomsList);

                //Console.WriteLine(i + " " + room.X + " " + room.Y + " " + room.Width + " " + room.Height); 
            }

            AsciiPrint(roomsList);

            return _map;
        }

        public MapHandler CreateMapBSP()
        {
            TreeNode<Room> root = new TreeNode<Room>(new Room
            {
                X = 0,
                Y = 0,
                Width = _width,
                Height = _height
            });

            var roomsTree = PartitionMapBSP(root, 15, 15);
            MakeRoomsBSP(roomsTree);

            return _map;
        }

        private TreeNode<Room> PartitionMapBSP(TreeNode<Room> current, int minWidth, int minHeight, int dir = -1)
        {
            Room room = current.Value;
            if (dir == -1) dir = _rand.Next(2);

            if (dir % 2 == 0)
            {
                if (room.Width < minWidth)
                {
                    if (_rand.Next(room.Height) > minHeight)
                        return PartitionMapBSP(current, minWidth, minHeight, 1);
                    else
                        return current;
                }

                int split = _rand.Next(4, room.Width - 3);
                Room left = new Room
                {
                    X = room.X,
                    Y = room.Y,
                    Width = split,
                    Height = room.Height
                };
                var leftChild = PartitionMapBSP(new TreeNode<Room>(current, left), minWidth, minHeight);
                current.AddChild(leftChild);

                Room right = new Room
                {
                    X = room.X + split,
                    Y = room.Y,
                    Width = room.Width - split,
                    Height = room.Height
                };
                var rightChild = PartitionMapBSP(new TreeNode<Room>(current, right), minWidth, minHeight);
                current.AddChild(rightChild);
            }
            else
            {
                if (room.Height < minHeight)
                {
                    if (_rand.Next(room.Width) > minWidth)
                        return PartitionMapBSP(current, minWidth, minHeight, 0);
                    else
                        return current;
                }

                int split = _rand.Next(4, room.Height - 3);
                Room top = new Room
                {
                    X = room.X,
                    Y = room.Y,
                    Width = room.Width,
                    Height = split
                };
                var topChild = PartitionMapBSP(new TreeNode<Room>(current, top), minWidth, minHeight);
                current.AddChild(topChild);

                Room bottom = new Room
                {
                    X = room.X,
                    Y = room.Y + split,
                    Width = room.Width,
                    Height = room.Height - split
                };
                var bottomChild = PartitionMapBSP(new TreeNode<Room>(current, bottom), minWidth, minHeight);
                current.AddChild(bottomChild);
            }

            return current;
        }

        private IList<(int X, int Y)> MakeRoomsBSP(TreeNode<Room> root)
        {
            IList<(int X, int Y)> allocated = new List<(int X, int Y)>();
            IList<(int X, int Y)> connections = new List<(int X, int Y)>();

            foreach (var child in root.Children)
            {
                var suballocated = MakeRoomsBSP(child);

                if (suballocated.Count > 0)
                {
                    var point = suballocated[_rand.Next(suballocated.Count)];

                    connections.Add(point);
                    allocated = allocated.Concat(suballocated).ToList();
                }
            }

            if (root.Children.Count == 0)
            {
                Room boundary = root.Value;
                Room space = new Room
                {
                    Width = _rand.Next(4, boundary.Width),
                    Height = _rand.Next(4, boundary.Height)
                };
                space.X = _rand.Next(boundary.X, boundary.X + boundary.Width - space.Width);
                space.Y = _rand.Next(boundary.Y, boundary.Y + boundary.Height - space.Height);

                CreateRoom2(space);

                for (int i = 0; i < space.Width; i++)
                {
                    for (int j = 0; j < space.Height; j++)
                    {
                        allocated.Add((space.X + i, space.Y + j));
                    }
                }
            }
            else
            {
                var (x1, y1) = connections[0];
                var (x2, y2) = connections[1];
                CreateHallway(x1, y1, x2, y2);
            }

            return allocated;
        }

        internal void AsciiPrint(IList<Room> rooms)
        {
            using (var writer = new System.IO.StreamWriter("map"))
            {
                for (int a = 0; a < _height; a++)
                {
                    for (int b = 0; b < _width; b++)
                    {
                        if (_map.Field[b, a].IsWalkable)
                        {
                            writer.Write(".");
                        }
                        else
                        {
                            writer.Write("#");
                        }
                    }

                    writer.WriteLine();
                }
            }
        }

        private void CreateRoom(Rectangle rect)
        {
            bool explored = true;

            for (int x = rect.Left; x < rect.Right; x++)
            {
                for (int y = rect.Top; y < rect.Bottom; y++)
                {
                    _map.SetCellProperties(x, y, true, true, explored);
                    _map.Field[x, y].IsWall = false;
                }
            }
        }

        private void CreateRoom2(Room room)
        {
            int x = room.X;
            int y = room.Y;

            for (int i = 1; i < room.Width; i++)
            {
                for (int j = 1; j < room.Height; j++)
                {
                    if (_map.Field.IsValid(x + i, y + j))
                    {
                        _map.SetCellProperties(x + i, y + j, true, true, true);
                        _map.Field[x + i, y + j].IsWall = false;
                    }
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

        private double RandNormal(double mean, double stdDev)
        {
            //uniform(0,1] random doubles
            double u1 = 1.0 - _rand.NextDouble();
            double u2 = 1.0 - _rand.NextDouble();

            //random normal(0,1)
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            //random normal(mean,stdDev^2)
            double randNormal = mean + stdDev * randStdNormal;

            return randNormal;
        }
    }
}
