using RLNET;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Items;

namespace Roguelike.Systems
{
    static class LookHandler
    {
        private static Actor _displayActor;
        private static Item _displayItem;
        private static Terrain _displayTile;
        private static bool _showActorInfo;
        private static bool _showItemInfo;
        private static bool _showTile;

        public static void DisplayActor(Actor actor)
        {
            _displayActor = actor;
            _showActorInfo = (actor != null);
            _showItemInfo = false;
        }

        public static void DisplayItem(ItemCount itemCount)
        {
            _displayItem = itemCount?.Item;
            _showItemInfo = (itemCount?.Item != null) && (itemCount.Count > 0);
            _showActorInfo = false;
        }

        internal static void DisplayTerrain(Terrain tile)
        {
            _displayTile = tile;
            _showTile = (tile != null);
        }

        public static void Draw(RLConsole console)
        {
            if (_showItemInfo)
                console.Print(1, 1, _displayItem.Name, Colors.TextHeading);

            if (_showActorInfo)
            {
                console.Print(1, 1, _displayActor.Name, Colors.TextHeading);
                console.Print(1, 2, "HP: " + _displayActor.Hp + " / " + _displayActor.Parameters.MaxHp, Colors.TextHeading);
                console.Print(1, 3, "MP: " + _displayActor.Mp + " / " + _displayActor.Parameters.MaxMp, Colors.TextHeading);
                console.Print(1, 4, "SP: " + _displayActor.Sp + " / " + _displayActor.Parameters.MaxSp, Colors.TextHeading);
                console.Print(1, 5, "Energy: " + _displayActor.Energy.ToString(), Colors.TextHeading);
                console.Print(1, 6, "State: " + _displayActor.State, Colors.TextHeading);
            }

            if (_showTile)
            {
                // console.Print(1, 8, "Move cost: " + _displayTile.MoveCost.ToString(), Colors.TextHeading);
                console.Print(1, 9, "Occupied: " + _displayTile.IsOccupied, Colors.TextHeading);
                console.Print(1, 9, "Walkable: " + _displayTile.IsWalkable, Colors.TextHeading);
                console.Print(1, 10, "Wall: " + _displayTile.IsWall, Colors.TextHeading);
                console.Print(1, 11, $"Position: ({_displayTile.X}, {_displayTile.Y})", Colors.TextHeading);
            }
        }
    }
}
