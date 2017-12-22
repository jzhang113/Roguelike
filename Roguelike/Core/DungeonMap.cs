using RLNET;
using RogueSharp;

namespace Roguelike.Core
{
    class DungeonMap : Map
    {
        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                Cell oldCell = GetCell(actor.X, actor.Y);
                SetCellProperties(oldCell.X, oldCell.Y, oldCell.IsTransparent, true, oldCell.IsExplored);

                actor.X = x;
                actor.Y = y;

                Cell newCell = GetCell(actor.X, actor.Y);
                SetCellProperties(newCell.X, newCell.Y, newCell.IsTransparent, false, newCell.IsExplored);

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
            mapConsole.Clear();

            foreach (Cell cell in GetAllCells())
            {
                DrawCell(mapConsole, cell);
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
        }
    }
}
