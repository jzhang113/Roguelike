using BearLib;
using Optional;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Items;
using Roguelike.Systems;
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
        internal int[,] Clearance { get; }

        // internal transient helper structures
        internal float[,] PlayerMap { get; }
        internal float[,] AutoexploreMap { get; }
        internal ICollection<Tile> Discovered { get; }

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

        // keep queue to prevent unnecessary allocations
        private readonly Queue<WeightedPoint> _goals = new Queue<WeightedPoint>();

        public MapHandler(int width, int height)
        {
            Width = width;
            Height = height;

            Field = new Field(width, height);
            Clearance = new int[width, height];
            PlayerMap = new float[width, height];
            AutoexploreMap = new float[width, height];
            Discovered = new List<Tile>();

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
            Clearance = (int[,])info.GetValue(nameof(Clearance), typeof(int[,]));

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
            Discovered = new List<Tile>();
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

        // Recalculate the state of the world after movements happen. If only light recalculations
        // needed, call UpdatePlayerFov() instead.
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

        public Option<Actor> GetActor(int x, int y) =>
            Units.TryGetValue(ToIndex(x, y), out Actor actor) ? Option.Some(actor) : Option.None<Actor>();

        public bool RemoveActor(Actor unit)
        {
            if (!Units.Remove(ToIndex(unit.X, unit.Y)))
                return false;

            Tile unitTile = Field[unit.X, unit.Y];
            unitTile.IsOccupied = false;
            unitTile.BlocksLight = false;

            unit.State = ActorState.Dead;
            Game.EventScheduler.RemoveActor(unit);
            return true;
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            //if (!Field[x, y].IsWalkable)
            //    return false;

            Field[actor.X, actor.Y].IsOccupied = false;
            Field[actor.X, actor.Y].BlocksLight = false;
            Units.Remove(ToIndex(actor.X, actor.Y));

            actor.X = x;
            actor.Y = y;
            Field[x, y].IsOccupied = true;
            Field[x, y].BlocksLight = actor.BlocksLight;
            Units.Add(ToIndex(x, y), actor);

            return true;
        }

        public Option<LevelId> TryChangeLocation(Actor actor) =>
            Exits.TryGetValue(ToIndex(actor.X, actor.Y), out Exit exit)
                ? Option.Some(exit.Destination)
                : Option.None<LevelId>();
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

        public Option<Item> GetItem(int x, int y)
        {
            bool found = Items.TryGetValue(ToIndex(x, y), out InventoryHandler stack);
            return (found && stack.Count > 0) ? Option.Some(stack.First()) : Option.None<Item>();
        }

        public Option<InventoryHandler> GetStack(int x, int y)
        {
            bool found = Items.TryGetValue(ToIndex(x, y), out InventoryHandler stack);
            return (found && stack.Count > 0) ? Option.Some(stack) : Option.None<InventoryHandler>();
        }

        // Clean up the items list by removing empty stacks.
        internal bool RemoveStackIfEmpty(int x, int y)
        {
            int index = ToIndex(x, y);
            if (!Items.TryGetValue(index, out InventoryHandler stack))
                return false;

            if (!stack.IsEmpty())
                return false;

            Items.Remove(index);
            return true;
        }

        // Take items off of the map.
        public Option<Item> SplitItem(Item item)
        {
            int index = ToIndex(item.X, item.Y);
            if (Items.TryGetValue(index, out InventoryHandler stack))
            {
                System.Diagnostics.Debug.Assert(
                    stack.Contains(item),
                    $"Map does not contain {item.Name}.");

                Item split = stack.Split(item, item.Count);
                if (stack.IsEmpty())
                    Items.Remove(index);

                return Option.Some(split);
            }
            else
            {
                System.Diagnostics.Debug.Fail($"Could not split {item.Name} on the map.");
                return Option.None<Item>();
            }
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
        #endregion

        public bool AddExit(Exit exit)
        {
            // TODO: also check exit reachability
            if (!Field[exit.X, exit.Y].IsWalkable)
                return false;

            Exits.Add(ToIndex(exit.X, exit.Y), exit);
            return true;
        }

        public Option<Exit> GetExit(int x, int y) =>
            Exits.TryGetValue(ToIndex(x, y), out Exit exit) ? Option.Some(exit) : Option.None<Exit>();

        public IEnumerable<Exit> GetExits(LevelId levelId) =>
            Exits.Values.Where(ex => ex.Destination == levelId);

        public bool SetFire(int x, int y)
        {
            int index = ToIndex(x, y);
            if (Fires.TryGetValue(index, out _))
                return false;

            Tile tile = Field[x, y];
            if (tile.IsWall)
                return false;

            TerrainProperty terrain = tile.Type.ToProperty();
            if (Game.Random.NextDouble() < terrain.Flammability.ToIgniteChance())
            {
                Fire fire = new Fire(x, y);
                Fires.Add(index, fire);
                Game.EventScheduler.AddActor(fire);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveFire(Fire fire)
        {
            if (!Fires.Remove(ToIndex(fire.X, fire.Y)))
                return false;

            Game.EventScheduler.RemoveActor(fire);
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

        internal WeightedPoint MoveTowardsTarget(int currentX, int currentY, float[,] goalMap, bool openDoors = false)
        {
            int nextX = currentX;
            int nextY = currentY;
            float nearest = goalMap[currentX, currentY];

            foreach ((int dx, int dy) in Direction.DirectionList)
            {
                int newX = currentX + dx;
                int newY = currentY + dy;

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

        public IEnumerable<Tile> GetStraightPathToPlayer(int x, int y)
        {
            System.Diagnostics.Debug.Assert(Field.IsValid(x, y));
            return Field[x, y].IsVisible
                ? GetStraightLinePath(x, y, Game.Player.X, Game.Player.Y)
                : new List<Tile>();
        }

        // Returns a straight line from the source to target. Does not check if the path is actually
        // walkable.
        public IEnumerable<Tile> GetStraightLinePath(int sourceX, int sourceY, int targetX, int targetY)
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
                    err -= dy;
                    sourceX += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    sourceY += sy;
                }

                yield return Field[sourceX, sourceY];
            }
        }

        public IEnumerable<Tile> GetTilesInRadius(int x, int y, int radius)
        {
            // square circles
            for (int i = x - radius; i <= x + radius; i++)
            {
                for (int j = y - radius; j <= y + radius; j++)
                {
                    if (Field.IsValid(i, j))
                        yield return Field[i, j];
                }
            }
        }

        public IEnumerable<Tile> GetTilesInRectangle(int x, int y, int width, int height)
        {
            for (int i = x; i <= x + width; i++)
            {
                for (int j = y; j <= y + height; j++)
                {
                    yield return Field[i, j];
                }
            }
        }

        public IEnumerable<Tile> GetTilesInRectangleBorder(int x, int y, int width, int height)
        {
            for (int i = x + 1; i < x + width; i++)
            {
                if (Field.IsValid(i, y))
                    yield return Field[i, y];

                if (Field.IsValid(i, y + height))
                    yield return Field[i, y + height];
            }

            for (int j = y; j <= y + height; j++)
            {
                if (Field.IsValid(x, j))
                    yield return Field[x, j];

                if (Field.IsValid(x + width, j))
                    yield return Field[x + width, j];
            }
        }

        // Octants are identified by the direction of the right edge. Returns a row starting from
        // the straight edge.
        private IEnumerable<Tile> GetRowInOctant(int x, int y, int distance, Dir dir)
        {
            if (dir == Direction.Center)
                yield break;

            for (int i = 0; i <= distance; i++)
            {
                int dx = 0;
                int dy = 0;

                if (dir == Direction.N)
                {
                    dx = -i;
                    dy = -distance;
                }
                else if (dir == Direction.NW)
                {
                    dx = -distance;
                    dy = -i;
                }
                else if (dir == Direction.W)
                {
                    dx = -distance;
                    dy = i;
                }
                else if (dir == Direction.SW)
                {
                    dx = -i;
                    dy = distance;
                }
                else if (dir == Direction.S)
                {
                    dx = i;
                    dy = distance;
                }
                else if (dir == Direction.SE)
                {
                    dx = distance;
                    dy = i;
                }
                else if (dir == Direction.E)
                {
                    dx = distance;
                    dy = -i;
                }
                else if (dir == Direction.NE)
                {
                    dx = i;
                    dy = -distance;
                }

                if (Field.IsValid(x + dx, y + dy))
                    yield return Field[x + dx, y + dy];
                else
                    yield break;
            }
        }
        #endregion

        #region FOV Methods
        public void ComputeFov(int x, int y, double lightDecay, bool setVisible)
        {
            foreach (Dir dir in Direction.DirectionList)
            {
                Queue<AngleRange> visibleRange = new Queue<AngleRange>();
                visibleRange.Enqueue(new AngleRange(1, 0, 1, 1));

                while (visibleRange.Count > 0)
                {
                    AngleRange range = visibleRange.Dequeue();
                    // If we don't care about setting visibility, we can stop once lightLevel reaches
                    // 0. Otherwise, we need to continue to check if los exists.
                    if (!setVisible && range.LightLevel < Constants.MIN_VISIBLE_LIGHT_LEVEL)
                        continue;

                    // There is really no need to check past 100 or something.
                    // TODO: put safeguards for when map gen borks and excavates the edges of the map
                    if (range.Distance > 100)
                        continue;

                    double delta = 0.5 / range.Distance;
                    IEnumerable<Tile> row = GetRowInOctant(x, y, range.Distance, dir);

                    CheckFovInRange(range, row, delta, visibleRange, lightDecay, setVisible);
                }
            }
        }

        // Sweep across a row and update the set of unblocked angles for the next row.
        private static void CheckFovInRange(in AngleRange range, IEnumerable<Tile> row, double delta,
            Queue<AngleRange> queue, double lightDecay, bool setVisible)
        {
            double currentAngle = 0;
            double newMinAngle = range.MinAngle;
            double newMaxAngle = range.MaxAngle;
            bool prevLit = false;
            bool first = true;

            foreach (Tile tile in row)
            {
                if (currentAngle > range.MaxAngle && Math.Abs(currentAngle - range.MaxAngle) > 0.001)
                {
                    // The line to the current tile falls outside the maximum angle. Partially
                    // light the tile and lower the maximum angle if we hit a wall.
                    double visiblePercent = (range.MaxAngle - currentAngle) / (2 * delta) + 0.5;
                    if (visiblePercent > 0)
                        tile.Light += (float)(visiblePercent * range.LightLevel);

                    if (setVisible)
                        tile.LosExists = true;

                    if (!tile.IsLightable)
                        newMaxAngle = currentAngle - delta;
                    break;
                }

                if (currentAngle > range.MinAngle || Math.Abs(currentAngle - range.MinAngle) < 0.001)
                {
                    double beginAngle = currentAngle - delta;
                    double endAngle = currentAngle + delta;

                    // Set the light level to the percent of tile visible. Note that tiles in a
                    // straight line from the center have their light values halved as each octant
                    // only covers half of the cells on the edges.
                    if (endAngle > range.MaxAngle)
                    {
                        double visiblePercent = (range.MaxAngle - currentAngle) / (2 * delta) + 0.5;
                        tile.Light += (float)(visiblePercent * range.LightLevel);
                    }
                    else if (beginAngle < range.MinAngle)
                    {
                        double visiblePercent = (currentAngle - range.MinAngle) / (2 * delta) + 0.5;
                        tile.Light += (float)(visiblePercent * range.LightLevel);
                    }
                    else
                    {
                        tile.Light += (float)range.LightLevel;
                    }

                    if (setVisible)
                    {
                        tile.LosExists = true;

                        // Since this method calculates LOS at the same time as player lighting,
                        // only set what the player can see as explored.
                        if (tile.Light > Constants.MIN_VISIBLE_LIGHT_LEVEL)
                            tile.IsExplored = true;
                    }

                    // For the first tile in a row, we only need to consider whether the current
                    // tile is blocked or not.
                    if (first)
                    {
                        first = false;
                        newMinAngle = !tile.IsLightable ? endAngle : range.MinAngle;
                    }
                    else
                    {
                        // If we are transitioning from an unblocked tile to a blocked tile, we need
                        // to lower the maximum angle for the next row.
                        if (prevLit && !tile.IsLightable)
                        {
                            int newDist = range.Distance + 1;
                            double light = range.LightLevel * (1 - lightDecay) * (1 - lightDecay);
                            queue.Enqueue(new AngleRange(newDist, newMinAngle, beginAngle, light));

                            // Update the minAngle to deal with single width walls in hallways.
                            newMinAngle = endAngle;
                        }
                        else if (!tile.IsLightable)
                        {
                            // If we are transitioning from a blocked tile to an unblocked tile, we
                            // need to raise the minimum angle.
                            newMinAngle = endAngle;
                        }
                    }
                }

                prevLit = tile.IsLightable;
                currentAngle += 2 * delta;
            }

            if (prevLit)
            {
                int newDist = range.Distance + 1;
                double light = range.LightLevel * (1 - lightDecay) * (1 - lightDecay);
                queue.Enqueue(new AngleRange(newDist, newMinAngle, newMaxAngle, light));
            }
        }

        private readonly struct AngleRange
        {
            internal int Distance { get; }
            internal double MinAngle { get; }
            internal double MaxAngle { get; }
            internal double LightLevel { get; }

            public AngleRange(int distance, double minAngle, double maxAngle, double lightLevel)
            {
                Distance = distance;
                MinAngle = minAngle;
                MaxAngle = maxAngle;
                LightLevel = lightLevel;
            }
        }
        #endregion

        #region Drawing Methods
        public void Draw(LayerInfo layer)
        {
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                LeftChar = '│', // 179
                RightChar = '│'
            });

            // draw everything else
            Terminal.Color(Colors.Text);
            for (int dx = 0; dx < layer.Width; dx++)
            {
                for (int dy = 0; dy < layer.Height; dy++)
                {
                    int newX = Camera.X + dx;
                    int newY = Camera.Y + dy;

                    if (newX >= Width || newY >= Height)
                        continue;

                    Tile tile = Field[newX, newY];
                    if (!tile.IsExplored)
                        continue;

                    if (tile.IsVisible)
                    {
                        tile.DrawingComponent.Draw(layer, tile);
                    }
                    else if (tile.IsWall)
                    {
                        Terminal.Color(Colors.WallBackground);
                        layer.Put(dx, dy, '#');
                    }
                    else
                    {
                        Terminal.Color(Colors.FloorBackground);
                        layer.Put(dx, dy, '.');
                    }
                }
            }

            foreach (Door door in Doors.Values)
            {
                door.DrawingComponent.Draw(layer, Field[door.X, door.Y]);
            }

            foreach (InventoryHandler stack in Items.Values)
            {
                Item topItem = stack.First();
                topItem.DrawingComponent.Draw(layer, Field[topItem.X, topItem.Y]);
            }

            foreach (Exit exit in Exits.Values)
            {
                exit.DrawingComponent.Draw(layer, Field[exit.X, exit.Y]);
            }

            foreach (Fire fire in Fires.Values)
            {
                fire.DrawingComponent.Draw(layer, Field[fire.X, fire.Y]);
            }

            foreach (Actor unit in Units.Values)
            {
                if (!unit.IsDead)
                {
                    unit.DrawingComponent.Draw(layer, Field[unit.X, unit.Y]);
                }
                else
                {
                    // HACK: draw some corpses
                    Terminal.Color(Swatch.DbOldBlood);
                    layer.Put(unit.X - Camera.X, unit.Y - Camera.Y, '%');
                }
            }
        }
        #endregion

        internal void UpdatePlayerFov()
        {
            // Clear vision from last turn
            // TODO: if we know the last move, we might be able to do an incremental update
            foreach (Tile tile in Field)
            {
                tile.Light = 0;
                tile.LosExists = false;
            }

            Discovered.Clear();

            Tile origin = Field[Game.Player.X, Game.Player.Y];
            origin.Light = 1;
            origin.LosExists = true;

            if (!origin.IsExplored)
            {
                origin.IsExplored = true;
                Discovered.Add(origin);
            }

            ComputeFov(Game.Player.X, Game.Player.Y, Constants.LIGHT_DECAY, true);
        }

        private void UpdatePlayerMaps()
        {
            _goals.Clear();
            _goals.Enqueue(new WeightedPoint(Game.Player.X, Game.Player.Y));

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    PlayerMap[x, y] = float.NaN;
                }
            }

            PlayerMap[Game.Player.X, Game.Player.Y] = 0;

            ProcessDijkstraMaps(_goals, PlayerMap);
        }

        private void UpdateAutoExploreMaps()
        {
            _goals.Clear();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Field[x, y].IsExplored)
                    {
                        AutoexploreMap[x, y] = float.NaN;
                    }
                    else
                    {
                        _goals.Enqueue(new WeightedPoint(x, y));
                        AutoexploreMap[x, y] = 0;
                    }
                }
            }

            ProcessDijkstraMaps(_goals, AutoexploreMap);
        }

        private void ProcessDijkstraMaps(Queue<WeightedPoint> goals, float[,] mapWeights)
        {
            while (goals.Count > 0)
            {
                WeightedPoint p = goals.Dequeue();

                foreach ((int dx, int dy) in Direction.DirectionList)
                {
                    int newX = p.X + dx;
                    int newY = p.Y + dy;
                    float newWeight = p.Weight + 1;

                    if (!Field.IsValid(newX, newY))
                        continue;

                    Tile tile = Field[newX, newY];

                    if (!tile.IsWall && tile.IsExplored &&
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
            info.AddValue(nameof(Clearance), Clearance);
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
