using RLNET;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Systems;
using Roguelike.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Roguelike.World
{
    [Serializable]
    public class MapHandler : ISerializable
    {
        public int Width { get; }
        public int Height { get; }

        internal Field Field { get; }

        // internal transient helper structures
        internal float[,] PlayerMap { get; }
        internal float[,] AutoexploreMap { get; }
        internal ICollection<Terrain> Discovered { get; }

        // can't auto serialize these dictionaries
        private IDictionary<int, Actor> Units { get; set; }
        private IDictionary<int, InventoryHandler> Items { get; set; }
        private IDictionary<int, Door> Doors { get; set; }
        private IDictionary<int, Exit> Exits { get; set; }
        private IDictionary<int, Fire> Fires { get; set; }

        private readonly KeyValueHelper<int, Actor> _tempUnits;
        private readonly KeyValueHelper<int, InventoryHandler> _tempItems;
        private readonly KeyValueHelper<int, Door> _tempDoors;
        private readonly KeyValueHelper<int, Exit> _tempExits;
        private readonly KeyValueHelper<int, Fire> _tempFires;

        public MapHandler(int width, int height)
        {
            Width = width;
            Height = height;

            Field = new Field(width, height);
            PlayerMap = new float[width, height];
            AutoexploreMap = new float[width, height];
            Discovered = new List<Terrain>();

            Units = new Dictionary<int, Actor>();
            Items = new Dictionary<int, InventoryHandler>();
            Doors = new Dictionary<int, Door>();
            Exits = new Dictionary<int, Exit>();
            Fires = new Dictionary<int, Fire>();
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
            _tempExits = new KeyValueHelper<int, Exit>
            {
                Key = (ICollection<int>)info.GetValue($"{nameof(Exits)}.keys", typeof(ICollection<int>)),
                Value = (ICollection<Exit>)info.GetValue($"{nameof(Exits)}.values", typeof(ICollection<Exit>))
            };
            _tempFires = new KeyValueHelper<int, Fire>
            {
                Key = (ICollection<int>)info.GetValue($"{nameof(Fires)}.keys", typeof(ICollection<int>)),
                Value = (ICollection<Fire>)info.GetValue($"{nameof(Fires)}.values", typeof(ICollection<Fire>))
            };

            PlayerMap = new float[Width, Height];
            AutoexploreMap = new float[Width, Height];
            Discovered = new List<Terrain>();
        }

        [OnDeserialized]
        protected void AfterDeserialize(StreamingContext context)
        {
            Units = _tempUnits.ToDictionary();
            Items = _tempItems.ToDictionary();
            Doors = _tempDoors.ToDictionary();
            Exits = _tempExits.ToDictionary();
            Fires = _tempFires.ToDictionary();

            foreach (Actor actor in Units.Values)
            {
                if (actor is Player player)
                    Game.Player = player;
            }

            foreach (Actor actor in Units.Values)
                Game.EventScheduler.AddActor(actor);

            foreach (Fire fire in Fires.Values)
                Game.EventScheduler.AddActor(fire);

            Refresh();
        }
        #endregion

        internal void Refresh()
        {
            Camera.UpdateCamera();
            UpdatePlayerFov();
            UpdatePlayerMaps();
            UpdateAutoExploreMaps();
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
            if (!Units.Remove(ToIndex(unit.X, unit.Y)))
                return false;

            Terrain unitTile = Field[unit.X, unit.Y];
            unitTile.IsOccupied = false;
            unitTile.BlocksLight = false;

            unit.State = Enums.ActorState.Dead;
            Game.EventScheduler.RemoveActor(unit);
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

        public bool TryChangeLocation(Actor actor, out LevelId destination)
        {
            if (Exits.TryGetValue(ToIndex(actor.X, actor.Y), out Exit exit))
            {
                destination = exit.Destination;
                return true;
            }

            destination = Game.World.CurrentLevel;
            return false;
        }

        #endregion

        #region Item Methods
        public void AddItem(ItemCount itemCount)
        {
            int index = ToIndex(itemCount.Item.X, itemCount.Item.Y);
            if (Items.TryGetValue(index, out InventoryHandler stack))
            {
                stack.Add(itemCount);
            }
            else
            {
                Items.Add(index, new InventoryHandler
                {
                    itemCount
                });
            }
        }

        public bool TryGetItem(int x, int y, out ItemCount itemCount)
        {
            bool success = Items.TryGetValue(ToIndex(x, y), out InventoryHandler stack);
            if (!success || stack.Count == 0)
            {
                itemCount = null;
                return false;
            }

            itemCount = stack.First();
            return true;
        }

        public bool TryGetStack(int x, int y, out InventoryHandler stack)
        {
            return Items.TryGetValue(ToIndex(x, y), out stack);
        }

        // Take an entire stack of item off of the map.
        public bool RemoveItem(ItemCount itemCount)
        {
            int index = ToIndex(itemCount.Item.X, itemCount.Item.Y);
            if (!Items.TryGetValue(index, out InventoryHandler stack))
                return false;

            if (!stack.Contains(itemCount))
                return false;

            stack.Remove(itemCount);
            if (stack.IsEmpty())
                Items.Remove(index);
            return true;
        }

        // Take only part of a stack from the map.
        public ItemCount SplitItem(ItemCount itemCount)
        {
            int index = ToIndex(itemCount.Item.X, itemCount.Item.Y);
            if (!Items.TryGetValue(index, out InventoryHandler stack))
            {
                System.Diagnostics.Debug.Assert(false, $"Could not split {itemCount.Item.Name} on the map.");
                return null;
            }

            System.Diagnostics.Debug.Assert(stack.Contains(itemCount), $"Map does not contain {itemCount.Item.Name}.");

            ItemCount split = stack.Split(itemCount);
            if (stack.IsEmpty())
                Items.Remove(index);
            return split;
        }
        #endregion

        #region Door Methods
        public bool AddDoor(Door door)
        {
            if (!Field[door.X, door.Y].IsWalkable)
                return false;

            Doors.Add(ToIndex(door.X, door.Y), door);
            Field[door.X, door.Y].IsOccupied = true;
            Field[door.X, door.Y].BlocksLight = door.BlocksLight;

            return true;
        }

        public bool TryGetDoor(int x, int y, out Door door)
        {
            return Doors.TryGetValue(ToIndex(x, y), out door);
        }

        public void OpenDoor(Door door)
        {
            door.DrawingComponent.Symbol = '-';
            door.IsOpen = true;
            Field[door.X, door.Y].IsOccupied = false;
            Field[door.X, door.Y].BlocksLight = false;
        }
        #endregion

        public bool AddExit(Exit exit)
        {
            // TODO: also check exit reachability
            if (!Field[exit.X, exit.Y].IsWalkable)
                return false;

            Exits.Add(ToIndex(exit.X, exit.Y), exit);
            return true;
        }

        public bool TryGetExit(int x, int y, out Exit exit)
        {
            return Exits.TryGetValue(ToIndex(x, y), out exit);
        }

        public bool SetFire(int x, int y)
        {
            int index = ToIndex(x, y);
            if (Fires.TryGetValue(index, out _))
                return false;

            if (Field[x, y].IsWall)
                return false;

            Fire fire = new Fire(x, y);
            Fires.Add(index, fire);
            Game.EventScheduler.AddActor(fire);
            return true;
        }

        public bool RemoveFire(Fire fire)
        {
            if (!Fires.Remove(ToIndex(fire.X, fire.Y)))
                return false;

            Game.EventScheduler.RemoveActor(fire);
            return true;
        }

        // Flammable objects have a chance to get set on fire.
        internal void ProcessFire(Fire fire)
        {
            int index = ToIndex(fire.X, fire.Y);

            if (Doors.TryGetValue(index, out Door door))
            {
                // TODO: create an actor burning implementation
                // Q: do doors behave like items or actors when burned?
                double burnChance;

                switch (Materials.MaterialList[door.Parameters.Material].Flammability)
                {
                    case Flammability.Low: burnChance = Constants.LOW_BURN_PERCENT; break;
                    case Flammability.Medium: burnChance = Constants.MEDIUM_BURN_PERCENT; break;
                    case Flammability.High: burnChance = Constants.HIGH_BURN_PERCENT; break;
                    default: burnChance = 0; break;
                }

                if (Game.World.Random.NextDouble() < burnChance)
                    OpenDoor(door);
            }

            if (Items.TryGetValue(index, out InventoryHandler stack))
            {
                stack.SetFire();
                if (stack.IsEmpty())
                    Items.Remove(index);
            }
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

        internal WeightedPoint MoveTowardsTarget(int currentX, int currentY, float[,] goalMap, bool openDoors = false)
        {
            int nextX = currentX;
            int nextY = currentY;
            float nearest = goalMap[currentX, currentY];

            foreach (WeightedPoint dir in Direction.Directions)
            {
                int newX = currentX + dir.X;
                int newY = currentY + dir.Y;

                if (Field.IsValid(newX, newY) && goalMap[newX, newY] < nearest)
                {
                    //if (Field[newX, newY].IsWalkable || Math.Abs(goalMap[newX, newY]) < 0.001f)
                    {
                        nextX = newX;
                        nextY = newY;
                        nearest = goalMap[newX, newY];
                    }
                    //else if (openDoors && TryGetDoor(newX, newY, out _))
                    //{
                    //    nextX = newX;
                    //    nextY = newY;
                    //    nearest = goalMap[newX, newY];
                    //}
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
                    if (Distance.EuclideanDistanceSquared(i, j, x, y) <= radius * radius)
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
            Discovered.Clear();

            // NOTE: slow implementation of fov - replace if needed
            // Set the player square to true and then skip it on future ray traces.
            Terrain playerTile = Field[x, y];
            playerTile.IsVisible = true;
            if (!playerTile.IsExplored)
            {
                playerTile.IsExplored = true;
                Discovered.Add(playerTile);
            }

            foreach (Terrain tile in GetTilesInRectangleBorder(x - radius / 2, y - radius / 2, radius, radius))
            {
                bool visible = true;
                foreach (Terrain tt in GetStraightLinePath(x, y, tile.X, tile.Y))
                {
                    tt.IsVisible = visible;
                    if (visible)
                    {
                        // Update the explored status while we're at it
                        if (!tt.IsExplored)
                        {
                            tt.IsExplored = true;
                            Discovered.Add(tt);
                        }

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
            for (int dx = 0; dx < Game.Config.MapView.Width; dx++)
            {
                for (int dy = 0; dy < Game.Config.MapView.Height; dy++)
                {
                    DrawTile(mapConsole, Camera.X + dx, Camera.Y + dy, dx, dy);
                }
            }

            foreach (Door door in Doors.Values)
            {
                int destX = door.X - Camera.X;
                int destY = door.Y - Camera.Y;
                door.DrawingComponent.Draw(mapConsole, Field[door.X, door.Y], destX, destY);
            }
            
            foreach (InventoryHandler stack in Items.Values)
            {
                Item topItem = stack.First().Item;
                int destX = topItem.X - Camera.X;
                int destY = topItem.Y - Camera.Y;
                topItem.DrawingComponent.Draw(mapConsole, Field[topItem.X, topItem.Y], destX, destY);
            }

            foreach (Exit exit in Exits.Values)
            {
                int destX = exit.X - Camera.X;
                int destY = exit.Y - Camera.Y;
                exit.DrawingComponent.Draw(mapConsole, Field[exit.X, exit.Y], destX, destY);
            }

            foreach (Fire fire in Fires.Values)
            {
                int destX = fire.X - Camera.X;
                int destY = fire.Y - Camera.Y;
                fire.DrawingComponent.Draw(mapConsole, Field[fire.X, fire.Y], destX, destY);
            }

            foreach (Actor unit in Units.Values)
            {
                if (!unit.IsDead)
                {
                    int destX = unit.X - Camera.X;
                    int destY = unit.Y - Camera.Y;
                    unit.DrawingComponent.Draw(mapConsole, Field[unit.X, unit.Y], destX, destY);
                }
                else
                    // HACK: draw some corpses
                    mapConsole.SetChar(unit.X - Camera.X, unit.Y - Camera.Y, '%');
            }
        }

        private void DrawTile(RLConsole mapConsole, int srcX, int srcY, int destX, int destY)
        {
            Terrain tile = Field[srcX, srcY];
            if (!tile.IsExplored)
                return;

            if (tile.IsVisible)
            {
                if (!tile.IsWall)
                {
                    mapConsole.Set(destX, destY, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                    // mapConsole.SetColor(tile.X, tile.Y, new RLColor(1, 1 - PlayerMap[tile.X, tile.Y] / 20, 0));
                }
                else
                {
                    mapConsole.Set(destX, destY, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            else
            {
                if (!tile.IsWall)
                {
                    mapConsole.Set(destX, destY, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    mapConsole.Set(destX, destY, Colors.Wall, Colors.WallBackground, '#');
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
            ComputeFov(player.X, player.Y, 100);
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

            ProcessDijkstraMaps(goals, PlayerMap);
        }

        private void UpdateAutoExploreMaps()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    AutoexploreMap[x, y] = float.NaN;
                }
            }

            Queue<WeightedPoint> goals = new Queue<WeightedPoint>();
            foreach (Terrain tile in Field)
            {
                if (!tile.IsExplored)
                {
                    goals.Enqueue(new WeightedPoint(tile.X, tile.Y));
                    AutoexploreMap[tile.X, tile.Y] = 0;
                }
            }

            ProcessDijkstraMaps(goals, AutoexploreMap);
        }

        private void ProcessDijkstraMaps(Queue<WeightedPoint> goals, float[,] mapWeights)
        {
            while (goals.Count > 0)
            {
                WeightedPoint p = goals.Dequeue();

                foreach (WeightedPoint dir in Direction.Directions)
                {
                    int newX = p.X + dir.X;
                    int newY = p.Y + dir.Y;
                    float newWeight = p.Weight + dir.Weight;
                    Terrain tile = Field[newX, newY];

                    if (Field.IsValid(newX, newY) && !tile.IsWall &&
                        (double.IsNaN(mapWeights[newX, newY]) || newWeight < mapWeights[newX, newY]))
                    {
                        mapWeights[newX, newY] = newWeight;
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
            info.AddValue(nameof(Fires), Fires);

            // Can't serialize Dictionary or KeyCollection, have to make it a list.
            info.AddValue($"{nameof(Units)}.keys", Units.Keys.ToList());
            info.AddValue($"{nameof(Units)}.values", Units.Values.ToList());
            info.AddValue($"{nameof(Items)}.keys", Items.Keys.ToList());
            info.AddValue($"{nameof(Items)}.values", Items.Values.ToList());
            info.AddValue($"{nameof(Doors)}.keys", Doors.Keys.ToList());
            info.AddValue($"{nameof(Doors)}.values", Doors.Values.ToList());
            info.AddValue($"{nameof(Exits)}.keys", Exits.Keys.ToList());
            info.AddValue($"{nameof(Exits)}.values", Exits.Values.ToList());
            info.AddValue($"{nameof(Fires)}.keys", Fires.Keys.ToList());
            info.AddValue($"{nameof(Fires)}.values", Fires.Values.ToList());
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
