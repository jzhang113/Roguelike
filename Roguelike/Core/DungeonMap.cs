using RLNET;
using Roguelike.Interfaces;
using Roguelike.Systems;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Core
{
    class DungeonMap : Map
    {
        internal RLColor[,] Highlight { get; set; }
        internal Terrain[,] Field { get; set; }
        internal float[,] PlayerMap { get; }
        internal float[,] FleeMap { get; }
        internal ICollection<Actor> Units { get; }

        public DungeonMap(int width, int height) : base(width, height)
        {
            Highlight = new RLColor[width, height];
            Field = new Terrain[width, height];
            PlayerMap = new float[width, height];
            FleeMap = new float[width, height];
            Units = new List<Actor>();
        }

        public bool AddActor(Actor unit)
        {
            if (!Field[unit.X, unit.Y].IsWalkable)
                return false;
            
            SetActorPosition(unit, unit.X, unit.Y);
            Units.Add(unit);
            Game.EventScheduler.AddActor(unit);

            return true;
        }

        public Actor GetActor(Cell cell)
        {
            return Units.FirstOrDefault(unit => unit.X == cell.X && unit.Y == cell.Y);
        }

        public bool RemoveActor(Actor unit)
        {
            SetWalkable(unit.X, unit.Y, true);
            unit.State = ActorState.Dead;
            Game.EventScheduler.RemoveActor(unit);

            return Units.Remove(unit);
        }

        public bool SetActorPosition(IActor actor, int x, int y)
        {
            Cell newPos = GetCell(x, y);

            if (newPos.IsWalkable)
            {
                SetWalkable(actor.X, actor.Y, true);

                actor.X = x;
                actor.Y = y;
                SetWalkable(newPos, false);

                if (actor is Player)
                {
                    UpdatePlayerFov();
                    UpdatePlayerMaps();
                }

                return true;
            }

            return false;
        }

        public IEnumerable<WeightedPoint> PathToPlayer(int x, int y)
        {
            float nearest = PlayerMap[x, y];

            while (nearest > 1.5f)
            {
                WeightedPoint nextMove = MoveTowardsTarget(x, y, PlayerMap);
                x = nextMove.X;
                y = nextMove.Y;
                nearest = nextMove.Weight;

                yield return nextMove;
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

                if (goalMap[newX, newY] < nearest)
                {
                    nextX = newX;
                    nextY = newY;
                    nearest = goalMap[newX, newY];
                }
            }

            return new WeightedPoint(nextX, nextY, nearest);
        }

        public void Draw(RLConsole mapConsole)
        {
            foreach (Cell cell in GetAllCells())
            {
                DrawCell(mapConsole, cell);
            }

            foreach (Actor unit in Units)
            {
                unit.Draw(mapConsole, this);
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
                if (cell.IsWalkable)
                {
                    mapConsole.Set(cell.X, cell.Y, Colors.FloorFov, Colors.FloorBackgroundFov, '.');
                    mapConsole.SetColor(cell.X, cell.Y, new RLColor(1, 1 - PlayerMap[cell.X, cell.Y] / 20, 0));
                }
                else
                {
                    mapConsole.Set(cell.X, cell.Y, Colors.WallFov, Colors.WallBackgroundFov, '#');
                }
            }
            else
            {
                if (cell.IsWalkable)
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

        private void UpdatePlayerMaps()
        {
            Queue<WeightedPoint> goals = new Queue<WeightedPoint>();
            bool[,] visited = new bool[Width, Height];
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

                    if (cell.IsWalkable && lx.IsExplored && (!visited[newX, newY] || newWeight < PlayerMap[newX, newY]))
                    {
                        visited[newX, newY] = true;
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

        private void SetWalkable(Cell cell, bool walkable)
        {
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, walkable, cell.IsExplored);
            Field[cell.X, cell.Y].IsWalkable = walkable;
        }

        private void SetWalkable(int x, int y, bool walkable)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, walkable, cell.IsExplored);
            Field[cell.X, cell.Y].IsWalkable = walkable;
        }
    }
}
