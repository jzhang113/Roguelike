using RLNET;
using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Core;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Roguelike.Systems
{
    [Serializable]
    public class MapHandler : ISerializable
    {
        public int Width { get; }
        public int Height { get; }

        internal Field Field { get; }

        internal RLColor[,] Highlight { get; }
        internal RLColor[,] PermHighlight { get; }
        internal float[,] PlayerMap { get; }

        // can't auto serialize these dictionaries
        private IDictionary<int, Actor> Units { get; set; }
        private IDictionary<int, InventoryHandler> Items { get; set; }
        private IDictionary<int, Door> Doors { get; set; }
        private IDictionary<int, Stair> Exits { get; set; }

        private readonly KeyValueHelper<int, Actor> _tempUnits;
        private readonly KeyValueHelper<int, InventoryHandler> _tempItems;
        private readonly KeyValueHelper<int, Door> _tempDoors;
        private readonly KeyValueHelper<int, Stair> _tempExits;

        public MapHandler(int width, int height)
        {
            Width = width;
            Height = height;

            Field = new Field(width, height);
            Highlight = new RLColor[width, height];
            PermHighlight = new RLColor[width, height];
            PlayerMap = new float[width, height];

            Units = new Dictionary<int, Actor>();
            Items = new Dictionary<int, InventoryHandler>();
            Doors = new Dictionary<int, Door>();
            Exits = new Dictionary<int, Stair>();
        }

        #region Serialization Constructor
        protected MapHandler(SerializationInfo info, StreamingContext context)
        {
            Width = info.GetInt32(nameof(Width));
            Height = info.GetInt32(nameof(Height));
            Field = (Field)info.GetValue(nameof(Field), typeof(Field));

            _tempUnits = new KeyValueHelper<int, Actor>
            {
                Key = (ICollection<int>)info.GetValue($"{nameof(Units)}.keys", typeof(ICollection<int>)),
                Value = (ICollection<Actor>)info.GetValue($"{nameof(Units)}.values", typeof(ICollection<Actor>))
            };
            _tempItems = new KeyValueHelper<int, InventoryHandler>
            {
                Key = (ICollection<int>)info.GetValue($"{nameof(Items)}.keys", typeof(ICollection<int>)),
                Value = (ICollection<InventoryHandler>)info.GetValue($"{nameof(Items)}.values", typeof(ICollection<InventoryHandler>))
            };
            _tempDoors = new KeyValueHelper<int, Door>
            {
                Key = (ICollection<int>)info.GetValue($"{nameof(Doors)}.keys", typeof(ICollection<int>)),
                Value = (ICollection<Door>)info.GetValue($"{nameof(Doors)}.values", typeof(ICollection<Door>))
            };
            _tempExits = new KeyValueHelper<int, Stair>
            {
                Key = (ICollection<int>)info.GetValue($"{nameof(Exits)}.keys", typeof(ICollection<int>)),
                Value = (ICollection<Stair>)info.GetValue($"{nameof(Exits)}.values", typeof(ICollection<Stair>))
            };

            Highlight = new RLColor[Width, Height];
            PermHighlight = new RLColor[Width, Height];
            PlayerMap = new float[Width, Height];
        }

        [OnDeserialized]
        protected void AfterDeserialize(StreamingContext context)
        {
            Units = _tempUnits.ToDictionary();
            Items = _tempItems.ToDictionary();
            Doors = _tempDoors.ToDictionary();
            Exits = _tempExits.ToDictionary();

            foreach (Actor actor in Units.Values)
            {
                if (actor is Player player)
                {
                    Game.Player = player;
                }
            }

            foreach (Actor actor in Units.Values)
                Game.EventScheduler.AddActor(actor);

            Refresh();
        }
        #endregion

        internal void Refresh()
        {
            UpdatePlayerFov();
            UpdatePlayerMaps();
        }

        #region Actor Methods
        public bool AddActor(Actor unit)
        {
            if (!Field[unit.X, unit.Y].IsWalkable)
                return false;

            SetActorPosition(unit, unit.X, unit.Y);
            Game.EventScheduler.AddActor(unit);

            return true;
        }

        public bool TryGetActor(int x, int y, out Actor actor)
        {
            return Units.TryGetValue(ToIndex(x, y), out actor);
        }

        public bool RemoveActor(Actor unit)
        {
            bool success = Units.Remove(ToIndex(unit.X, unit.Y));
            if (!success)
                return false;

            unit.State = Enums.ActorState.Dead;
            Field[unit.X, unit.Y].IsOccupied = false;
            Field[unit.X, unit.Y].BlocksLight = false;

            return true;
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (!Field[x, y].IsWalkable)
                return false;

            Field[actor.X, actor.Y].IsOccupied = false;
            Field[actor.X, actor.Y].BlocksLight = false;
            Units.Remove(ToIndex(actor.X, actor.Y));

            actor.X = x;
            actor.Y = y;
            Field[x, y].IsOccupied = true;
            Field[x, y].BlocksLight = actor.BlocksLight;
            Units.Add(ToIndex(x, y), actor);

            if (actor is Player)
                Refresh();

            return true;
        }

        public bool TryChangeLocation(Actor actor, out string destination)
        {
            if (Exits.TryGetValue(ToIndex(actor.X, actor.Y), out Stair exit))
            {
                destination = exit.Destination;
                return true;
            }

            destination = null;
            return false;
        }

        #endregion

        #region Item Methods
        public void AddItem(Item item)
        {
            int index = ToIndex(item.X, item.Y);
            if (Items.TryGetValue(index, out InventoryHandler stack))
            {
                stack.Add(item);
            }
            else
            {
                Items.Add(index, new InventoryHandler
                {
                    item
                });
            }
        }

        public bool TryGetItem(int x, int y, out Item item)
        {
            bool success = Items.TryGetValue(ToIndex(x, y), out InventoryHandler stack);
            if (!success || stack.Count == 0)
            {
                item = null;
                return false;
            }

            item = stack.First();
            return true;
        }

        public bool TryGetStack(int x, int y, out InventoryHandler stack)
        {
            return Items.TryGetValue(ToIndex(x, y), out stack);
        }

        // Take an entire stack of item off of the map.
        public bool RemoveItem(Item item)
        {
            int index = ToIndex(item.X, item.Y);
            if (!Items.TryGetValue(index, out InventoryHandler stack))
                return false;

            if (!stack.Contains(item))
                return false;

            return stack.Remove(item);
            // Q: remove stack from Items if it is empty?
        }

        // Permanently remove items from the map.
        public bool DestroyItem(Item item, int amount)
        {
            int index = ToIndex(item.X, item.Y);
            if (!Items.TryGetValue(index, out InventoryHandler stack))
                return false;

            if (!stack.Contains(item))
                return false;

            stack.Destroy(item, amount);
            return true;
        }

        // Take only part of a stack from the map.
        public Item SplitItem(Item item, int amount)
        {
            int index = ToIndex(item.X, item.Y);
            if (!Items.TryGetValue(index, out InventoryHandler stack))
            {
                System.Diagnostics.Debug.Assert(false, $"Could not split {item.Name} on the map.");
                return null;
            }

            System.Diagnostics.Debug.Assert(stack.Contains(item), $"Map does not contain {item.Name}.");

            if (amount < item.Count)
                return stack.Split(item, amount);

            bool removeStatus = stack.Remove(item);
            System.Diagnostics.Debug.Assert(removeStatus, $"Could not remove {item.Name} at {index}.");

            return item;
        }
        #endregion

        #region Door Methods
        public bool AddDoor(Door door)
        {
            if (!Field[door.X, door.Y].IsWalkable)
                return false;

            Doors.Add(ToIndex(door.X, door.Y), door);
            Units.Add(ToIndex(door.X, door.Y), door);
            Field[door.X, door.Y].IsOccupied = true;
            Field[door.X, door.Y].BlocksLight = door.BlocksLight;

            return true;
        }

        public void OpenDoor(Door door)
        {
            door.DrawingComponent.Symbol = '-';
            Units.Remove(ToIndex(door.X, door.Y));
            Field[door.X, door.Y].IsOccupied = false;
            Field[door.X, door.Y].BlocksLight = false;
        }
        #endregion

        public bool AddExit(Stair exit)
        {
            // TODO: also check exit reachability
            if (!Field[exit.X, exit.Y].IsWalkable)
                return false;

            Exits.Add(ToIndex(exit.X, exit.Y), exit);
            return true;
        }

        #region Tile Selection Methods
        public IEnumerable<WeightedPoint> GetPathToPlayer(int x, int y)
        {
            System.Diagnostics.Debug.Assert(Field.IsValid(x, y));
            float nearest = PlayerMap[x, y];
            float prev = nearest;

            while (nearest > 0)
            {
                WeightedPoint nextMove = MoveTowardsTarget(x, y, PlayerMap);
                x = nextMove.X;
                y = nextMove.Y;
                nearest = nextMove.Weight;

                if (Math.Abs(nearest - prev) < 0.001f || Math.Abs(nearest) < 0.01f)
                {
                    yield break;
                }
                else
                {
                    prev = nearest;
                    yield return nextMove;
                }
            }
        }

        internal WeightedPoint MoveTowardsTarget(int currentX, int currentY, float[,] goalMap)
        {
            int nextX = currentX;
            int nextY = currentY;
            float nearest = goalMap[currentX, currentY];

            foreach (WeightedPoint dir in Direction.Directions)
            {
                int newX = currentX + dir.X;
                int newY = currentY + dir.Y;

                if (Field.IsValid(newX, newY) && goalMap[newX, newY] < nearest &&
                    (Field[newX, newY].IsWalkable || Math.Abs(goalMap[newX, newY]) < 0.001f))
                {
                    nextX = newX;
                    nextY = newY;
                    nearest = goalMap[newX, newY];
                }
            }

            return new WeightedPoint(nextX, nextY, nearest);
        }

        public IEnumerable<Terrain> GetStraightPathToPlayer(int x, int y)
        {
            System.Diagnostics.Debug.Assert(Field.IsValid(x, y));
            return Field[x, y].IsVisible ? GetStraightLinePath(x, y, Game.Player.X, Game.Player.Y) : new List<Terrain>();
        }

        // Returns a straight line from the source to target. Does not check if the path is actually
        // walkable.
        public IEnumerable<Terrain> GetStraightLinePath(int sourceX, int sourceY, int targetX, int targetY)
        {
            int dx = Math.Abs(targetX - sourceX);
            int dy = Math.Abs(targetY - sourceY);
            int sx = (targetX < sourceX) ? -1 : 1;
            int sy = (targetY < sourceY) ? -1 : 1;
            int err = dx - dy;

            // Skip initial position?
            // yield return Field[sourceX, sourceY];

            // Take a step towards the target and return the new position.
            while (sourceX != targetX || sourceY != targetY)
            {
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    sourceX += sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    sourceY += sy;
                }

                yield return Field[sourceX, sourceY];
            }
        }

        public IEnumerable<Terrain> GetTilesInRadius(int x, int y, int radius)
        {
            // NOTE: lazy implementation - replace if needed
            for (int i = x - radius; i <= x + radius; i++)
            {
                for (int j = y - radius; j <= y + radius; j++)
                {
                    if (Utils.Distance.EuclideanDistanceSquared(i, j, x, y) <= radius * radius)
                        yield return Field[i, j];
                }
            }
        }

        public IEnumerable<Terrain> GetTilesInRectangle(int x, int y, int width, int height)
        {
            for (int i = x; i <= x + width; i++)
            {
                for (int j = y; j <= y + height; j++)
                {
                    yield return Field[i, j];
                }
            }
        }

        public IEnumerable<Terrain> GetTilesInRectangleBorder(int x, int y, int width, int height)
        {
            for (int i = x + 1; i < x + width; i++)
            {
                yield return Field[i, y];
                yield return Field[i, y + height];
            }

            for (int j = y; j <= y + height; j++)
            {
                yield return Field[x, j];
                yield return Field[x + width, j];
            }
        }
        #endregion

        #region FOV Methods
        public void ComputeFov(int x, int y, int radius)
        {
            // NOTE: slow implementation of fov - replace if needed
            // Set the player square to true and then skip it on future ray traces.
            Field[x, y].IsVisible = true;
            Field[x, y].IsExplored = true;

            foreach (Terrain tile in GetTilesInRectangleBorder(x - radius / 2, y - radius / 2, radius, radius))
            {
                bool visible = true;
                foreach (Terrain tt in GetStraightLinePath(x, y, tile.X, tile.Y))
                {
                    tt.IsVisible = visible;
                    if (visible)
                    {
                        // Update the explored status while we're at it
                        tt.IsExplored = true;

                        if (!tt.IsLightable)
                            visible = false;
                    }
                }
            }

            foreach (Terrain tile in GetTilesInRectangle(x - radius / 2, y - radius / 2, radius, radius))
            {
                // TODO: post processing to remove visual artifacts.
            }
        }
        #endregion

        #region Drawing Methods
        public void Draw(RLConsole mapConsole)
        {
            foreach (Terrain tile in Field)
            {
                DrawTile(mapConsole, tile);
            }

            foreach (Door door in Doors.Values)
            {
                door.DrawingComponent.Draw(mapConsole, this);
            }

            foreach (InventoryHandler stack in Items.Values)
            {
                if (stack.Any())
                    stack.First().DrawingComponent.Draw(mapConsole, this);
            }

            foreach (Actor unit in Units.Values)
            {
                if (!unit.IsDead)
                    unit.DrawingComponent.Draw(mapConsole, this);
                else
                    // HACK: draw some corpses
                    mapConsole.SetChar(unit.X, unit.Y, '%');
            }

            foreach (Stair exit in Exits.Values)
            {
                exit.DrawingComponent.Draw(mapConsole, this);
            }

            // debugging code for dijkstra maps
            //foreach (Terrain tile in Field)
            //{
            //    if (Game.ShowOverlay)
            //        DrawOverlay(mapConsole, tile);
            //}
        }

        private void DrawTile(RLConsole mapConsole, Terrain tile)
        {
            if (!tile.IsExplored)
                return;

            if (Field[tile.X, tile.Y].IsVisible)
            {
                if (!Field[tile.X, tile.Y].IsWall)
                {
                    mapConsole.Set(tile.X, tile.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                    // mapConsole.SetColor(tile.X, tile.Y, new RLColor(1, 1 - PlayerMap[tile.X, tile.Y] / 20, 0));
                }
                else
                {
                    mapConsole.Set(tile.X, tile.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            else
            {
                if (!Field[tile.X, tile.Y].IsWall)
                {
                    mapConsole.Set(tile.X, tile.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    mapConsole.Set(tile.X, tile.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }

            mapConsole.SetBackColor(tile.X, tile.Y, Highlight[tile.X, tile.Y]);
        }

        private void DrawOverlay(RLConsole mapConsole, Terrain tile)
        {
            string display;
            float distance = PlayerMap[tile.X, tile.Y];

            if (distance < 10 || float.IsNaN(distance))
                display = distance.ToString(CultureInfo.InvariantCulture);
            else
                display = ((char)(distance - 10 + 'a')).ToString();

            mapConsole.Print(tile.X, tile.Y, display, display == "NaN" ? Swatch.DbBlood : Swatch.DbWater);
        }

        internal void ClearHighlight()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Highlight[x, y] = RLColor.Black;
                }
            }
        }
        #endregion

        private void UpdatePlayerFov()
        {
            // Clear vision from last turn
            foreach (Terrain tile in Field)
                tile.IsVisible = false;

            Player player = Game.Player;
            ComputeFov(player.X, player.Y, player.Awareness);
        }

        private void UpdatePlayerMaps()
        {
            Queue<WeightedPoint> goals = new Queue<WeightedPoint>();
            goals.Enqueue(new WeightedPoint(Game.Player.X, Game.Player.Y));

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    PlayerMap[x, y] = float.NaN;
                }
            }

            foreach (WeightedPoint p in goals)
            {
                PlayerMap[p.X, p.Y] = 0;
            }

            while (goals.Count > 0)
            {
                WeightedPoint p = goals.Dequeue();

                foreach (WeightedPoint dir in Direction.Directions)
                {
                    int newX = p.X + dir.X;
                    int newY = p.Y + dir.Y;
                    float newWeight = p.Weight + dir.Weight;
                    Terrain tile = Field[newX, newY];

                    if (Field.IsValid(newX, newY) && !tile.IsWall && tile.IsExplored &&
                        (float.IsNaN(PlayerMap[newX, newY]) || newWeight < PlayerMap[newX, newY]))
                    {
                        PlayerMap[newX, newY] = newWeight;
                        goals.Enqueue(new WeightedPoint(newX, newY, newWeight));
                    }
                }
            }
        }

        private int ToIndex(int x, int y)
        {
            return x + Width * y;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Width), Width);
            info.AddValue(nameof(Height), Height);
            info.AddValue(nameof(Field), Field);

            // Can't serialize Dictionary or KeyCollection, have to make it a list.
            info.AddValue($"{nameof(Units)}.keys", Units.Keys.ToList());
            info.AddValue($"{nameof(Units)}.values", Units.Values.ToList());
            info.AddValue($"{nameof(Items)}.keys", Items.Keys.ToList());
            info.AddValue($"{nameof(Items)}.values", Items.Values.ToList());
            info.AddValue($"{nameof(Doors)}.keys", Doors.Keys.ToList());
            info.AddValue($"{nameof(Doors)}.values", Doors.Values.ToList());
            info.AddValue($"{nameof(Exits)}.keys", Exits.Keys.ToList());
            info.AddValue($"{nameof(Exits)}.values", Exits.Values.ToList());
        }

        // Temporarily store lists when deserializing dictionaries
        private class KeyValueHelper<TKey, TValue>
        {
            public ICollection<TKey> Key { private get; set; }
            public ICollection<TValue> Value { private get; set; }

            public IDictionary<TKey, TValue> ToDictionary()
            {
                return Key.Zip(Value, (k, v) => new { Key = k, Value = v })
                    .ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
