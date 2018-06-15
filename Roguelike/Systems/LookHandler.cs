using RLNET;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Items;

namespace Roguelike.Systems
{
    class LookHandler
    {
        private static Actor _displayActor;
        private static ItemInfo _displayItem;
        private static Terrain _displayTile;
        private static bool _showActorInfo;
        private static bool _showItemInfo;
        private static bool _showTile;

        public static void DisplayActor(Actor actor)
        {
            _displayActor = actor;
            _showActorInfo = (actor != null);
        }

        public static void DisplayItem(ItemInfo itemGroup)
        {
            _displayItem = itemGroup;
            _showItemInfo = (itemGroup != null) && (itemGroup.Count > 0);
        }

        internal static void DisplayTerrain(Terrain tile)
        {
            _displayTile = tile;
            _showTile = (tile != null);
        }

        public static void Draw(RLConsole console)
        {
            if (_showItemInfo)
                console.Print(1, 1, _displayItem?.Item.Name, Colors.TextHeading);

            if (_showActorInfo)
            {
                console.Print(1, 1, _displayActor.Name, Colors.TextHeading);
                console.Print(1, 2, "HP: " + _displayActor.HP + " / " + _displayActor.MaxHP, Colors.TextHeading);
                console.Print(1, 3, "MP: " + _displayActor.MP + " / " + _displayActor.MaxMP, Colors.TextHeading);
                console.Print(1, 4, "SP: " + _displayActor.SP + " / " + _displayActor.MaxSP, Colors.TextHeading);
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
