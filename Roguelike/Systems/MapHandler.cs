using RLNET;
using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Core;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Systems
{
    [Serializable]
    public class MapHandler : ISerializable
    {
        public int Width { get; }
        public int Height { get; }

        internal RLColor[,] Highlight { get; }
        internal RLColor[,] PermHighlight { get; }
        internal Field Field { get; }
        internal float[,] PlayerMap { get; }

        // Internal so that we can add Actors to the EventScheduler when deserializing.
        internal ICollection<Actor> Units { get; }
        private ICollection<ItemInfo> Items { get; }
        private ICollection<Door> Doors { get; }

        public MapHandler(int width, int height)
        {
            Width = width;
            Height = height;

            Field = new Field(width, height);
            Highlight = new RLColor[width, height];
            PermHighlight = new RLColor[width, height];
            PlayerMap = new float[width, height];

            Units = new List<Actor>();
            Items = new List<ItemInfo>();
            Doors = new List<Door>();
        }

        protected MapHandler(SerializationInfo info, StreamingContext context)
        {
            Width = (int)info.GetValue(nameof(Width), typeof(int));
            Height = (int)info.GetValue(nameof(Height), typeof(int));
            Field = (Field)info.GetValue(nameof(Field), typeof(Field));
            Units = (ICollection<Actor>)info.GetValue(nameof(Units), typeof(ICollection<Actor>));
            Items = (ICollection<ItemInfo>)info.GetValue(nameof(Items), typeof(ICollection<ItemInfo>));
            Doors = (ICollection<Door>)info.GetValue(nameof(Doors), typeof(ICollection<Door>));

            Highlight = new RLColor[Width, Height];
            PlayerMap = new float[Width, Height];

            Refresh();
        }

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
            Units.Add(unit);
            Field[unit.X, unit.Y].Unit = unit;
            Game.EventScheduler.AddActor(unit);

            return true;
        }

        public Actor GetActor(int x, int y)
        {
            return Field[x, y].Unit;
        }

        public void RemoveActor(Actor unit)
        {
            bool success = Units.Remove(unit);
            if (success)
            {
                unit.State = Enums.ActorState.Dead;
                Field[unit.X, unit.Y].Unit = null;
                Game.EventScheduler.RemoveActor(unit);
            }
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (Field[x, y].IsWalkable)
            {
                Field[actor.X, actor.Y].Unit = null;

                actor.X = x;
                actor.Y = y;
                Field[x, y].Unit = actor;

                if (actor is Player)
                    Refresh();

                return true;
            }

            return false;
        }
        #endregion

        #region Item Methods
        public bool AddItem(ItemInfo itemGroup)
        {
            bool found = false;

            foreach (ItemInfo stack in Items)
            {
                if (stack.Equals(itemGroup))
                {
                    stack.Add(itemGroup.Count);
                    found = true;
                }
            }

            if (!found)
                Items.Add(itemGroup);

            if (Field[itemGroup.Item.X, itemGroup.Item.Y].ItemStack == null)
                Field[itemGroup.Item.X, itemGroup.Item.Y].ItemStack = new InventoryHandler();

            Field[itemGroup.Item.X, itemGroup.Item.Y].ItemStack.Add(itemGroup);
            return true;
        }

        public void RemoveItem(ItemInfo itemGroup)
        {
            foreach (ItemInfo stack in Items)
            {
                if (stack.Equals(itemGroup))
                    stack.Remove(itemGroup.Count);
            }

            Field[itemGroup.Item.X, itemGroup.Item.Y].ItemStack.Remove(itemGroup);
        }

        public ItemInfo GetItem(int x, int y)
        {
            return Items.FirstOrDefault(item => item.Item.X == x && item.Item.Y == y && item.Count > 0);
        }
        #endregion

        #region Door Methods
        public bool AddDoor(Door door)
        {
            if (!Field[door.X, door.Y].IsWalkable)
                return false;

            Doors.Add(door);
            Field[door.X, door.Y].Unit = door;

            return true;
        }

        public void OpenDoor(Door door)
        {
            door.Symbol = '-';
            Doors.Remove(door);
        }
        #endregion

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

                if (nearest == prev || nearest == 0)
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

                if (goalMap[newX, newY] < nearest && (Field[newX, newY].IsWalkable || goalMap[newX, newY] == 0))
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
            if (Field[x, y].IsVisible)
                return GetStraightLinePath(x, y, Game.Player.X, Game.Player.Y);
            else
                return new List<Terrain>();
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

            // Return the initial position.
            yield return Field[sourceX, sourceY];

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
                    if (Field[i, j] != null && Utils.Distance.EuclideanDistanceSquared(i, j, x, y) <= radius * radius)
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
            foreach (Terrain tile in GetTilesInRectangleBorder(x - radius / 2, y - radius / 2, radius, radius))
            {
                bool visible = true;
                foreach (Terrain tt in GetStraightLinePath(x, y, tile.X, tile.Y).Skip(1))
                {
                    tt.IsVisible = visible;
                    if (visible && tt.BlocksLight)
                        visible = false;
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

            foreach (ItemInfo stack in Items)
            {
                if (stack.Count > 0)
                    stack.Item.Draw(mapConsole, this);
            }

            foreach (Actor unit in Units)
            {
                if (!unit.IsDead)
                    unit.Draw(mapConsole, this);
            }

            foreach (Door door in Doors)
            {
                door.Draw(mapConsole, this);
            }

            // debugging code for dijkstra maps
            foreach (Terrain tile in Field)
            {
                if (Game.ShowOverlay)
                    DrawOverlay(mapConsole, tile);
            }

        }

        private void DrawTile(RLConsole mapConsole, Terrain tile)
        {
            if (!tile.IsExplored)
            {
                return;
            }

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
                display = distance.ToString();
            else
                display = ((char)(distance - 10 + 'a')).ToString();

            if (display == "NaN")
                mapConsole.Print(tile.X, tile.Y, display, Swatch.DbBlood);
            else
                mapConsole.Print(tile.X, tile.Y, display, Swatch.DbWater);
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
            Player player = Game.Player;
            ComputeFov(player.X, player.Y, player.Awareness);

            foreach (Terrain tile in Field)
            {
                if (Field[tile.X, tile.Y].IsVisible)
                {
                    tile.IsExplored = true;
                }
            }
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

                    if (tile != null && !tile.IsWall && tile.IsExplored &&
                        (float.IsNaN(PlayerMap[newX, newY]) || newWeight < PlayerMap[newX, newY]))
                    {
                        PlayerMap[newX, newY] = newWeight;
                        goals.Enqueue(new WeightedPoint(newX, newY, newWeight));
                    }
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Width), Width);
            info.AddValue(nameof(Height), Height);
            info.AddValue(nameof(Field), Field);
            info.AddValue(nameof(Units), Units);
            info.AddValue(nameof(Items), Items);
            info.AddValue(nameof(Doors), Doors);
        }
    }
}
