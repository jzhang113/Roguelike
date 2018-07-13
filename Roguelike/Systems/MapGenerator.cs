﻿using Roguelike.Actions;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using RLNET;
using Roguelike.World;
using Pcg;

namespace Roguelike.Systems
{
    class MapGenerator
    {
        private const int _MIN_ROOM_SIZE = 4;
        private const int _MAX_ROOM_SIZE = 15;

        private readonly int _width;
        private readonly int _height;
        private readonly IEnumerable<LevelId> _exits;
        private readonly PcgRandom _rand;
        private readonly MapHandler _map;

        public MapGenerator(int width, int height, IEnumerable<LevelId> exits, PcgRandom random)
        {
            _width = width;
            _height = height;
            _exits = exits;
            _rand = random;
            _map = new MapHandler(_width, _height);
        }

        public MapHandler CreateMap(RegionType regionType)
        {
            switch (regionType)
            {
                case RegionType.Root:
                case RegionType.Main:
                case RegionType.Side:
                case RegionType.Otherside:
                    return CreateMapBsp();
                default:
                    throw new NotImplementedException();
            }
        }

        private MapHandler FillMap()
        {
            IList<Room> roomsList = new List<Room>();
            int centerX = _width / 2;
            int centerY = _height / 2;

            for (int i = 0; i < 1000; i++)
            {
                Room room = new Room(
                    x: _rand.Next(centerX - 5, centerX + 5),
                    y: _rand.Next(centerY - 5, centerY + 5),
                    width: (int)_rand.NextNormal(5, 1),
                    height: (int)_rand.NextNormal(5, 1));

                bool first = true;
                bool horiz = false;
                bool intersects;
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

                            room.Offset((int)(multiplier * dx), (int)(multiplier * dy));
                        }
                    }
                } while (intersects && iterations < 100);

                roomsList.Add(room);
                CreateRoom(room);
                //AsciiPrint(roomsList);

                //Console.WriteLine(i + " " + room.X + " " + room.Y + " " + room.Width + " " + room.Height); 
            }

            AsciiPrint();

            return _map;
        }

        private MapHandler CreateMapBsp()
        {
            TreeNode<Room> root = new TreeNode<Room>(new Room(0, 0, _width, _height));
            TreeNode<Room> roomsPartition = PartitionMapBsp(root, _MAX_ROOM_SIZE, _MAX_ROOM_SIZE);
            ICollection<Room> roomsList = new List<Room>();

            MakeRoomsBsp(roomsPartition, ref roomsList);
            foreach (Room r in roomsList)
            {
                PlaceDoors(r);
            }

            PlaceActors();
            PlaceItems();
            PlaceStairs();

            return _map;
        }

        private TreeNode<Room> PartitionMapBsp(TreeNode<Room> current, int minWidth, int minHeight, int dir = -1)
        {
            Room room = current.Value;
            if (dir == -1) dir = _rand.Next(2);

            if (dir % 2 == 0)
            {
                if (room.Width < minWidth)
                {
                    return _rand.Next(room.Height) > minHeight
                        ? PartitionMapBsp(current, minWidth, minHeight, 1)
                        : current;
                }

                int split = _rand.Next(_MIN_ROOM_SIZE, room.Width - _MIN_ROOM_SIZE + 1);
                Room left = new Room(room.X, room.Y, split, room.Height);
                var leftChild = PartitionMapBsp(new TreeNode<Room>(current, left), minWidth, minHeight);
                current.AddChild(leftChild);

                Room right = new Room(room.X + split, room.Y, room.Width - split, room.Height);
                var rightChild = PartitionMapBsp(new TreeNode<Room>(current, right), minWidth, minHeight);
                current.AddChild(rightChild);
            }
            else
            {
                if (room.Height < minHeight)
                {
                    return _rand.Next(room.Width) > minWidth
                        ? PartitionMapBsp(current, minWidth, minHeight, 0)
                        : current;
                }

                int split = _rand.Next(_MIN_ROOM_SIZE, room.Height - _MIN_ROOM_SIZE + 1);
                Room top = new Room(room.X, room.Y, room.Width, split);
                var topChild = PartitionMapBsp(new TreeNode<Room>(current, top), minWidth, minHeight);
                current.AddChild(topChild);

                Room bottom = new Room(room.X, room.Y + split, room.Width, room.Height - split);
                var bottomChild = PartitionMapBsp(new TreeNode<Room>(current, bottom), minWidth, minHeight);
                current.AddChild(bottomChild);
            }

            return current;
        }

        private IList<(int X, int Y)> MakeRoomsBsp(TreeNode<Room> root, ref ICollection<Room> roomsList)
        {
            IList<(int X, int Y)> allocated = new List<(int X, int Y)>();
            IList<(int X, int Y)> connections = new List<(int X, int Y)>();

            foreach (var child in root.Children)
            {
                var suballocated = MakeRoomsBsp(child, ref roomsList);

                if (suballocated.Count <= 0)
                    continue;

                var point = suballocated[_rand.Next(suballocated.Count)];

                connections.Add(point);
                allocated = allocated.Concat(suballocated).ToList();
            }

            if (root.Children.Count == 0)
            {
                Room boundary = root.Value;
                Room space = new Room(
                    width: _rand.Next(4, boundary.Width),
                    height: _rand.Next(4, boundary.Height));
                space.X = _rand.Next(boundary.X, boundary.X + boundary.Width - space.Width);
                space.Y = _rand.Next(boundary.Y, boundary.Y + boundary.Height - space.Height);

                CreateRoom(space);
                roomsList.Add(space);

                // Add the tiles on the walls which can become doors
                for (int i = space.Left; i < space.Right - 1; i++)
                {
                    allocated.Add((i, space.Top + 1));
                    allocated.Add((i, space.Bottom - 1));
                }

                for (int j = space.Top; j < space.Bottom - 1; j++)
                {
                    allocated.Add((space.Left + 1, j));
                    allocated.Add((space.Right - 1, j));
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

        private void AsciiPrint()
        {
            using (var writer = new System.IO.StreamWriter("map"))
            {
                for (int a = 0; a < _height; a++)
                {
                    for (int b = 0; b < _width; b++)
                    {
                        writer.Write(_map.Field[b, a].IsWalkable ? "." : "#");
                    }

                    writer.WriteLine();
                }
            }
        }

        private void CreateRoom(Room room)
        {
            for (int i = room.Left; i < room.Right; i++)
            {
                for (int j = room.Top; j < room.Bottom; j++)
                {
                    // Don't excavate the edges of the map
                    if (i == 0 || j == 0 || i == _width - 1 || j == _height - 1)
                        continue;

                    _map.Field[i, j].IsWall = false;
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
                    CreateRoom(new Room(x2 - dx, y2, dx + 1, 1));
                    CreateRoom(new Room(x1, y1, 1, dy + 1));
                }
                else
                {
                    CreateRoom(new Room(x1, y1, dx + 1, 1));
                    CreateRoom(new Room(x2, y2, 1, dy + 1));
                }
            }
            else
            {
                if (y1 < y2)
                {
                    CreateRoom(new Room(x2, y2, dx + 1, 1));
                    CreateRoom(new Room(x1, y1, 1, dy + 1));
                }
                else
                {
                    CreateRoom(new Room(x1 - dx, y1, dx + 1, 1));
                    CreateRoom(new Room(x2, y2, 1, dy + 1));
                }
            }
        }

        private void PlaceDoors(Room room)
        {
            for (int i = room.Left; i < room.Right; i++)
            {
                if (i <= 0 || i >= _width - 1)
                    continue;

                if (room.Top > 1 && IsDoorLocation(i, room.Top - 1))
                {
                    _map.AddDoor(new Door
                    {
                        X = i,
                        Y = room.Top - 1
                    });
                }

                if (room.Bottom < _height - 1 && IsDoorLocation(i, room.Bottom))
                {
                    _map.AddDoor(new Door
                    {
                        X = i,
                        Y = room.Bottom
                    });
                }
            }

            for (int j = room.Top; j < room.Bottom; j++)
            {
                if (j <= 0 || j >= _height - 1)
                    continue;

                if (room.Left > 1 && IsDoorLocation(room.Left - 1, j))
                {
                    _map.AddDoor(new Door
                    {
                        X = room.Left - 1,
                        Y = j
                    });
                }

                if (room.Right < _width - 1 && IsDoorLocation(room.Right, j))
                {
                    _map.AddDoor(new Door
                    {
                        X = room.Right,
                        Y = j
                    });
                }
            }
        }

        private bool IsDoorLocation(int x, int y)
        {
            bool current = _map.Field[x, y].IsWall;
            bool left = _map.Field[x - 1, y].IsWall;
            bool right = _map.Field[x + 1, y].IsWall;
            bool up = _map.Field[x, y - 1].IsWall;
            bool down = _map.Field[x, y + 1].IsWall;

            if (current)
                return false;

            if (left && right && !up && !down)
                return true;

            if (!left && !right && up && down)
                return true;

            return false;
        }

        // HACK: ad-hoc placement code
        private void PlaceItems()
        {
            Weapon spear = new Weapon(
                new ItemParameter("spear", MaterialType.Wood)
                {
                    AttackSpeed = 240,
                    Damage = 200,
                    MeleeRange = 1.5f,
                    ThrowRange = 7
                }, Swatch.DbBlood)
            {
                X = Game.Player.X - 1,
                Y = Game.Player.Y - 1
            };
            _map.AddItem(new ItemCount { Item = spear, Count = 1 });

            IAction heal = new HealAction(100, new TargetZone(Enums.TargetShape.Self));

            //var lungeSkill = new List<IAction>()
            //{
            //    new MoveAction(new TargetZone(Enums.TargetShape.Directional)),
            //    new DamageAction(100, new TargetZone(Enums.TargetShape.Directional))
            //};
            spear.AddAbility(new DamageAction(100, new TargetZone(Enums.TargetShape.Directional)));

            Armor ha = new Armor(
                new ItemParameter("heavy armor", MaterialType.Iron)
                {
                    AttackSpeed = 1000,
                    Damage = 100,
                    MeleeRange = 1,
                    ThrowRange = 3
                }, Swatch.DbMetal, Enums.ArmorType.Armor)
            {
                X = Game.Player.X - 2,
                Y = Game.Player.Y - 3
            };
            _map.AddItem(new ItemCount { Item = ha, Count = 1 });

            _map.AddItem(new ItemCount
            {
                Item = new Scroll(
                    new ItemParameter("scroll of magic missile", MaterialType.Paper),
                    new DamageAction(
                        200,
                        new TargetZone(Enums.TargetShape.Directional, range: 10)),
                    Swatch.DbSun)
                    {
                        X = Game.Player.X - 1,
                        Y = Game.Player.Y - 2
                    },
                Count = 1
            });

            Scroll healing = new Scroll(
                new ItemParameter("scroll of healing", MaterialType.Paper), heal, Swatch.DbGrass)
            {
                X = Game.Player.X + 1,
                Y = Game.Player.Y + 1
            };
            _map.AddItem(new ItemCount { Item = healing, Count = 1 });

            _map.AddItem(new ItemCount
            {
                Item = new Scroll(
                    new ItemParameter("scroll of enchantment", MaterialType.Paper),
                    new EnchantAction(
                        new TargetZone(Enums.TargetShape.Range, range: 10)),
                    RLColor.LightGreen)
                {
                    X = Game.Player.X - 1,
                    Y = Game.Player.Y
                },
                Count = 1
            });

            _map.AddItem(new ItemCount
            {
                Item = new Scroll(
                    new ItemParameter("scroll of fireball", MaterialType.Paper),
                    new DamageAction(
                        200,
                        new TargetZone(Enums.TargetShape.Range, range: 10, radius: 3)),
                    Swatch.DbBlood)
                {
                    X = Game.Player.X - 2,
                    Y = Game.Player.Y - 2
                },
                Count = 1
            });


            Item planks = new Item(
                    new ItemParameter("plank", MaterialType.Wood), Swatch.DbWood, '\\')
            {
                X = Game.Player.X + 2,
                Y = Game.Player.Y + 2,
            };
            Item planks2 = new Item(planks) { X = Game.Player.X + 3 };
            Item planks3 = new Item(planks) { Y = Game.Player.Y + 3 };
            Item planks4 = new Item(planks) { X = Game.Player.X + 3, Y = Game.Player.Y + 3 };
            _map.AddItem(new ItemCount { Item = planks, Count = 10 });
            _map.AddItem(new ItemCount { Item = planks2, Count = 10 });
            _map.AddItem(new ItemCount { Item = planks3, Count = 10 });
            _map.AddItem(new ItemCount { Item = planks4, Count = 10 });
        }

        // HACK: ad-hoc placement code
        private void PlaceActors()
        {
            do
            {
                Game.Player.X = _rand.Next(1, Game.Config.Map.Width - 1);
                Game.Player.Y = _rand.Next(1, Game.Config.Map.Height - 1);
            }
            while (!_map.Field[Game.Player.X, Game.Player.Y].IsWalkable);

            _map.AddActor(Game.Player);
            // Map.SetActorPosition(Player, playerX, playerY);

            for (int i = 0; i < 3; i++)
            {
                Skeleton s = new Skeleton(new ActorParameters($"Skeleton #{i}")
                {
                    Awareness = 10,
                    MaxHp = 50,
                    MaxMp = 20,
                    MaxSp = 20
                });
                while (!_map.Field[s.X, s.Y].IsWalkable)
                {
                    s.X = _rand.Next(1, Game.Config.Map.Width - 1);
                    s.Y = _rand.Next(1, Game.Config.Map.Height - 1);
                }
                _map.AddActor(s);
            }

            _map.SetFire(Game.Player.X + 3, Game.Player.Y + 3);
        }

        private void PlaceStairs()
        {
            foreach (LevelId id in _exits)
            {
                Exit exit = new Exit(id);
                while (!_map.Field[exit.X, exit.Y].IsWalkable && !_map.TryGetExit(exit.X, exit.Y, out _))
                {
                    exit.X = _rand.Next(1, Game.Config.Map.Width - 1);
                    exit.Y = _rand.Next(1, Game.Config.Map.Height - 1);
                }

                _map.AddExit(exit);
            }
        }
    }
}
