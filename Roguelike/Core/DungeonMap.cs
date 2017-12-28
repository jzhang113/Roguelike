using RLNET;
using RogueSharp;
using System.Collections.Generic;
using System.Linq;

namespace Roguelike.Core
{
    class DungeonMap : Map
    {
        public bool[,] highlight;
        private ICollection<Actor> _units;

        public DungeonMap(int width, int height) : base(width, height)
        {
            highlight = new bool[width, height];
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

        public bool SetActorPosition(Actor actor, int x, int y)
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
                }

                return true;
            }

            return false;
        }

        public void UpdatePlayerFov()
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

            if (highlight[cell.X, cell.Y])
            {
                mapConsole.SetBackColor(cell.X, cell.Y, RLColor.Red);
            }
        }

        internal void ClearHighlight()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    highlight[i, j] = false;
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
