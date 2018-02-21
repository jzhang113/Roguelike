using RLNET;
using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Core;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Roguelike.Systems
{
    class MapHandler : Map
    {
        internal RLColor[,] Highlight { get; set; }
        internal Field Field { get; set; }
        internal float[,] PlayerMap { get; }
        internal float[,] FleeMap { get; }
        internal ICollection<Actor> Units { get; }
        internal ICollection<ItemInfo> Items { get; }
        
        public MapHandler(int width, int height) : base(width, height)
        {
            Field = new Field(width, height);
            Highlight = new RLColor[width, height];
            PlayerMap = new float[width, height];
            FleeMap = new float[width, height];
            Units = new List<Actor>();
            Items = new List<ItemInfo>();
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

        public Actor GetActor(WeightedPoint pos)
        {
            return Field[pos.X, pos.Y].Unit;
        }

        public Actor GetActor(int x, int y)
        {
            return Field[x, y].Unit;
        }

        public bool RemoveActor(Actor unit)
        {
            SetOccupied(unit.X, unit.Y, false);
            unit.State = ActorState.Dead;
            Field[unit.X, unit.Y].Unit = null;
            Game.EventScheduler.RemoveActor(unit);

            return Units.Remove(unit);
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            Cell newPos = GetCell(x, y);

            if (newPos.IsWalkable)
            {
                SetOccupied(actor.X, actor.Y, false);
                Field[actor.X, actor.Y].Unit = null;

                actor.X = x;
                actor.Y = y;
                SetOccupied(newPos, true);
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

        public ItemInfo GetItem(WeightedPoint pos)
        {
            return Items.FirstOrDefault(item => item.Item.X == pos.X && item.Item.Y == pos.Y && item.Count > 0);
        }

        public ItemInfo GetItem(int x, int y)
        {
            return Items.FirstOrDefault(item => item.Item.X == x && item.Item.Y == y && item.Count > 0);
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

                if (nearest == prev)
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

            foreach (WeightedPoint dir in Move.Directions)
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
            int D = 2 * dy - dx;

            // Return the initial position.
            yield return Field[sourceX, sourceY];

            // Take a step towards the target and return the new position.
            while (sourceX != targetX)
            {
                sourceX += sx;

                if (D > 0)
                {
                    sourceY += sy;
                    D -= 2 * dx;
                }

                D += 2 * dy;

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

            if (!Highlight[cell.X, cell.Y].Equals(RLColor.Black))
            {
                mapConsole.SetBackColor(cell.X, cell.Y, Highlight[cell.X, cell.Y]);
            }
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
                }
            }
        }

        internal void UpdatePlayerMaps()
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

                foreach (WeightedPoint dir in Move.Directions)
                {
                    int newX = p.X + dir.X;
                    int newY = p.Y + dir.Y;
                    float newWeight = p.Weight + dir.Weight;
                    Terrain cell = Field[newX, newY];
                    Cell lx = GetCell(newX, newY);

                    if (cell != null && !cell.IsWall && lx.IsExplored &&
                        (float.IsNaN(PlayerMap[newX, newY]) || newWeight < PlayerMap[newX, newY]))
                    {
                        PlayerMap[newX, newY] = newWeight;
                        goals.Enqueue(new WeightedPoint(newX, newY, newWeight));
                    }
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    FleeMap[x, y] = PlayerMap[x, y] * -1.2f;
                }
            }
        }

        private void SetOccupied(Cell cell, bool occupied)
        {
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, !occupied, cell.IsExplored);
            Field[cell.X, cell.Y].IsOccupied = occupied;
        }

        private void SetOccupied(int x, int y, bool occupied)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, !occupied, cell.IsExplored);
            Field[cell.X, cell.Y].IsOccupied = occupied;
        }
    }
}
