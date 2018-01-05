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
        internal float[,] PlayerDistance { get; }
        internal ICollection<Actor> Units { get; }

        public DungeonMap(int width, int height) : base(width, height)
        {
            Highlight = new RLColor[width, height];
            Field = new Terrain[width, height];
            PlayerDistance = new float[width, height];
            Units = new List<Actor>();
        }

        public bool AddActor(Actor unit)
        {
            if (!GetCell(unit.X, unit.Y).IsWalkable)
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
            float nearest = PlayerDistance[x, y];

            while (nearest > 1.5f)
            {
                WeightedPoint nextMove = MoveTowardsPlayer(x, y);
                x = nextMove.X;
                y = nextMove.Y;
                nearest = nextMove.Weight;

                yield return nextMove;
            }
        }

        internal WeightedPoint MoveTowardsPlayer(int currentX, int currentY)
        {
            int nextX = currentX;
            int nextY = currentY;
            float nearest = PlayerDistance[currentX, currentY];

            foreach (WeightedPoint dir in Move.Directions)
            {
                int newX = currentX + dir.X;
                int newY = currentY + dir.Y;

                if (PlayerDistance[newX, newY] < nearest)
                {
                    nextX = newX;
                    nextY = newY;
                    nearest = PlayerDistance[newX, newY];
                }
            }

            return new WeightedPoint(nextX, nextY, nearest);
        }

        internal WeightedPoint MoveAwayFromPlayer(int currentX, int currentY)
        {
            int nextX = currentX;
            int nextY = currentY;
            float farthest = PlayerDistance[currentX, currentY];

            foreach (WeightedPoint dir in Move.Directions)
            {
                int newX = currentX + dir.X;
                int newY = currentY + dir.Y;

                if (PlayerDistance[newX, newY] > farthest)
                {
                    nextX = newX;
                    nextY = newY;
                    farthest = PlayerDistance[newX, newY];
                }
            }

            return new WeightedPoint(nextX, nextY, farthest);
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
                    mapConsole.SetColor(cell.X, cell.Y, new RLColor(1, 1 - PlayerDistance[cell.X, cell.Y] / 40, 0));
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
                    PlayerDistance[x, y] = float.NaN;
                }
            }

            foreach (WeightedPoint p in goals)
            {
                PlayerDistance[p.X, p.Y] = 0;
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

                    if (cell.IsWalkable && (!visited[newX, newY] || newWeight < PlayerDistance[newX, newY]))
                    {
                        visited[newX, newY] = true;
                        PlayerDistance[newX, newY] = newWeight;
                        goals.Enqueue(new WeightedPoint(newX, newY, newWeight));
                    }
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
