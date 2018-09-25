using BearLib;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Items;

namespace Roguelike.Systems
{
    internal static class LookHandler
    {
        private static Actor _displayActor;
        private static Item _displayItem;
        private static Tile _displayTile;
        private static bool _showActorInfo;
        private static bool _showItemInfo;
        private static bool _showTile;

        public static void DisplayActor(Actor actor)
        {
            _displayActor = actor;
            _showActorInfo = true;
            _showItemInfo = false;
        }

        public static void DisplayItem(ItemCount itemCount)
        {
            _displayItem = itemCount.Item;
            _showItemInfo = true;
            _showActorInfo = false;
        }

        internal static void Clear()
        {
            _showActorInfo = false;
            _showItemInfo = false;
        }

        internal static void DisplayTerrain(Tile tile)
        {
            _displayTile = tile;
            _showTile = tile != null;
        }

        public static void Draw()
        {
            Terminal.Color(Colors.Text);

            if (_showItemInfo)
                Terminal.Print(1, 1, _displayItem.Name);

            if (_showActorInfo)
            {
                Terminal.Print(1, 1, _displayActor.Name);
                Terminal.Print(1, 2, $"HP: {_displayActor.Hp} / {_displayActor.Parameters.MaxHp}");
                Terminal.Print(1, 3, $"MP: {_displayActor.Mp} / {_displayActor.Parameters.MaxMp}");
                Terminal.Print(1, 4, $"SP: {_displayActor.Sp} / {_displayActor.Parameters.MaxSp}");
                Terminal.Print(1, 5, $"Energy: {_displayActor.Energy}");
                Terminal.Print(1, 6, $"State: {_displayActor.State}");
            }

            if (_showTile)
            {
                // console.Print(1, 8, $"Move cost: {_displayTile.MoveCost.ToString()}");
                Terminal.Print(1, 9, $"Occupied: {_displayTile.IsOccupied}");
                Terminal.Print(1, 9, $"Walkable: {_displayTile.IsWalkable}");
                Terminal.Print(1, 10, $"Wall: {_displayTile.IsWall}");
                Terminal.Print(1, 11, $"Position: ({_displayTile.X}, {_displayTile.Y})");
            }
        }
    }
}
