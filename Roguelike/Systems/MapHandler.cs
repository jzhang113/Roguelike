﻿using RLNET;
using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Core;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Systems
{
    // TODO: remove dependence on Map - implement FOV
    [Serializable]
    public class MapHandler : Map, ISerializable
    {
        public new int Width { get; }
        public new int Height { get; }

        internal RLColor[,] Highlight { get; set; }
        internal Field Field { get; set; }
        internal float[,] PlayerMap { get; }

        private ICollection<Actor> Units { get; }
        private ICollection<ItemInfo> Items { get; }
        private ICollection<Door> Doors { get; }
        
        public MapHandler()
        {

        }

        public MapHandler(int width, int height) : base(width, height)
        {
            Width = width;
            Height = height;

            Field = new Field(width, height);
            Highlight = new RLColor[width, height];
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

        public bool RemoveActor(Actor unit)
        {
            SetOccupied(unit.X, unit.Y, false);
            unit.State = Enums.ActorState.Dead;
            Field[unit.X, unit.Y].Unit = null;
            Game.EventScheduler.RemoveActor(unit);

            return Units.Remove(unit);
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (Field[x, y].IsWalkable)
            {
                SetOccupied(actor.X, actor.Y, false);
                Field[actor.X, actor.Y].Unit = null;

                actor.X = x;
                actor.Y = y;
                SetOccupied(x, y, true);
                Field[x, y].Unit = actor;

                if (actor is Player)
                {
                    UpdatePlayerFov();
                    UpdatePlayerMaps();
                }

                return true;
            }

            return false;
        }
        #endregion

        #region Item Methods
        public bool AddItem(Item item)
        {
            bool found = false;

            foreach (ItemInfo stack in Items)
            {
                if (stack.Contains(item))
                {
                    stack.Add();
                    found = true;
                }
            }

            if (!found)
                Items.Add(new ItemInfo(item));

            if (Field[item.X, item.Y].ItemStack == null)
                Field[item.X, item.Y].ItemStack = new InventoryHandler();

            Field[item.X, item.Y].ItemStack.Add(item);
            return true;
        }

        public void RemoveItem(Item item)
        {
            foreach (ItemInfo stack in Items)
            {
                if (stack.Contains(item))
                    stack.Remove();
            }

            Field[item.X, item.Y].ItemStack.Remove(item);
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

            Cell cell = GetCell(door.X, door.Y);
            SetCellProperties(cell.X, cell.Y, false, true, cell.IsExplored);

            return true;
        }

        public void OpenDoor(Door door)
        {
            door.Symbol = '-';
            Doors.Remove(door);
            
            Cell cell = GetCell(door.X, door.Y);
            SetCellProperties(cell.X, cell.Y, true, false, cell.IsExplored);
        }
        #endregion

        public IEnumerable<WeightedPoint> PathToPlayer(int x, int y)
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

        public IEnumerable<Terrain> StraightPathToPlayer(int x, int y)
        {
            System.Diagnostics.Debug.Assert(Field.IsValid(x, y));
            if (IsInFov(x, y))
                return StraightLinePath(x, y, Game.Player.X, Game.Player.Y);
            else
                return new List<Terrain>();
        }

        // Returns a straight line from the source to target. Does not check if the path is actually
        // walkable.
        internal IEnumerable<Terrain> StraightLinePath(int sourceX, int sourceY, int targetX, int targetY)
        {
            int dx = Math.Abs(targetX - sourceX);
            int dy = Math.Abs(targetY - sourceY);
            int sx = (targetX < sourceX) ? -1 : 1;
            int sy = (targetY < sourceY) ? -1 : 1;
            int err = dx - dy;

            // Return the initial position.
            yield return Field[sourceX, sourceY];

            // Take a step towards the target and return the new position.
            while (sourceX != targetX)
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

        #region Drawing Methods
        public void Draw(RLConsole mapConsole)
        {
            foreach (Cell cell in GetAllCells())
            {
                DrawCell(mapConsole, cell);
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
            foreach (Cell cell in GetAllCells())
            {
                if (Game.ShowOverlay)
                    DrawOverlay(mapConsole, cell);
            }

        }

        private void DrawCell(RLConsole mapConsole, Cell cell)
        {
            if (!cell.IsExplored)
            {
                return;
            }

            if (IsInFov(cell.X, cell.Y))
            {
                if (!Field[cell.X, cell.Y].IsWall)
                {
                    mapConsole.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                    // mapConsole.SetColor(cell.X, cell.Y, new RLColor(1, 1 - PlayerMap[cell.X, cell.Y] / 20, 0));
                }
                else
                {
                    mapConsole.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            else
            {
                if (!Field[cell.X, cell.Y].IsWall)
                {
                    mapConsole.Set(cell.X, cell.Y, Colors.Floor, Colors.FloorBackground, '.');
                }
                else
                {
                    mapConsole.Set(cell.X, cell.Y, Colors.Wall, Colors.WallBackground, '#');
                }
            }

            mapConsole.SetBackColor(cell.X, cell.Y, Highlight[cell.X, cell.Y]);
        }

        private void DrawOverlay(RLConsole mapConsole, Cell cell)
        {
            string display;
            float distance = PlayerMap[cell.X, cell.Y];

            if (distance < 10 || float.IsNaN(distance))
                display = distance.ToString();
            else
                display = ((char)(distance - 10 + 'a')).ToString();

            if (display == "NaN")
                mapConsole.Print(cell.X, cell.Y, display, Swatch.DbBlood);
            else
                mapConsole.Print(cell.X, cell.Y, display, Swatch.DbWater);
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
            ComputeFov(player.X, player.Y, player.Awareness, true);

            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                    Field[cell.X, cell.Y].IsExplored = true;
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
                    Terrain cell = Field[newX, newY];

                    if (cell != null && !cell.IsWall && cell.IsExplored &&
                        (float.IsNaN(PlayerMap[newX, newY]) || newWeight < PlayerMap[newX, newY]))
                    {
                        PlayerMap[newX, newY] = newWeight;
                        goals.Enqueue(new WeightedPoint(newX, newY, newWeight));
                    }
                }
            }
        }

        private void SetOccupied(int x, int y, bool occupied)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, !occupied, cell.IsExplored);
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
