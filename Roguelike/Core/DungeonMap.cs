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
        public bool[,] Highlight { get; set; }
        public float[,] PlayerDistance { get; set; }

        private ICollection<Actor> _units;

        public DungeonMap(int width, int height) : base(width, height)
        {
            Highlight = new bool[width, height];
            PlayerDistance = new float[width, height];
            _units = new List<Actor>();
        }

        public bool AddActor(Actor unit)
        {
            Cell pos = GetCell(unit.X, unit.Y);

            if (pos.IsWalkable)
            {
                SetWalkable(pos, false);
                _units.Add(unit);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Actor GetActor(Cell cell)
        {
            return _units.FirstOrDefault(unit => unit.X == cell.X && unit.Y == cell.Y);
        }

        public bool RemoveActor(Actor unit)
        {
            SetWalkable(unit.X, unit.Y, true);
            return _units.Remove(unit);
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

        public IEnumerable<Point> PathToPlayer(int x, int y)
        {
            int nextX = x;
            int nextY = y;
            float nearest = PlayerDistance[x, y];

            do
            {
                if (nearest < 1.5f)
                    break;

                x = nextX;
                y = nextY;

                foreach (WeightedPoint dir in Move.Directions)
                {
                    int newX = x + dir.X;
                    int newY = y + dir.Y;

                    if (PlayerDistance[newX, newY] < nearest)
                    {
                        nextX = newX;
                        nextY = newY;
                        nearest = PlayerDistance[newX, newY];
                    }
                }

                yield return new Point(nextX, nextY);
            } while (nearest != PlayerDistance[x, y]);
        }

        public void Draw(RLConsole mapConsole)
        {
            foreach (Cell cell in GetAllCells())
            {
                DrawCell(mapConsole, cell);
            }

            foreach (Actor unit in _units)
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
                    
                    mapConsole.SetColor(cell.X, cell.Y, new RLColor(PlayerDistance[cell.X, cell.Y], 0, 0));
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

            if (Highlight[cell.X, cell.Y])
            {
                mapConsole.SetBackColor(cell.X, cell.Y, RLColor.Red);
            }
        }

        internal void ClearHighlight()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Highlight[x, y] = false;
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
                    PlayerDistance[x, y] = float.MaxValue;
                }
            }

            while (goals.Count > 0)
            {
                WeightedPoint p = goals.Dequeue();

                foreach (WeightedPoint dir in Move.Directions)
                {
                    int newX = p.X + dir.X;
                    int newY = p.Y + dir.Y;
                    float newWeight = p.Weight + dir.Weight;

                    if (GetCell(newX, newY).IsWalkable && (!visited[newX, newY] || newWeight < PlayerDistance[newX, newY]))
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
        }

        private void SetWalkable(int x, int y, bool walkable)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, walkable, cell.IsExplored);
        }
    }
}
