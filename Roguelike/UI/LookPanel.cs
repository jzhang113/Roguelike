using BearLib;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Data;
using Roguelike.Items;

namespace Roguelike.UI
{
    internal static class LookPanel
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

        public static void DisplayItem(Item item)
        {
            _displayItem = item;
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

        public static void Draw(LayerInfo layer)
        {
            Game.ShowInfo = false;
            // draw borders
            Terminal.Color(Colors.BorderColor);
            layer.DrawBorders(new BorderInfo
            {
                TopLeftChar = '╔', // 201
                BottomLeftChar = '╚', // 200
                TopChar = '═', // 205
                BottomChar = '═',
                LeftChar = '║' // 186
            });
            layer.Print(-1, $"{Constants.HEADER_LEFT}[color=white]SCAN{Constants.HEADER_SEP}" +
                $"[/color]DATA{Constants.HEADER_RIGHT}", System.Drawing.ContentAlignment.TopRight);

            // draw info
            Terminal.Color(Colors.Text);
            if (_showItemInfo)
                layer.Print(1, _displayItem.Name);

            if (_showActorInfo)
            {
                layer.Print(1, _displayActor.Name);
                layer.Print(2, $"HP: {_displayActor.Hp} / {_displayActor.Parameters.MaxHp}");
                layer.Print(3, $"SP: {_displayActor.Sp} / {_displayActor.Parameters.MaxSp}");
                layer.Print(4, $"Energy: {_displayActor.Energy}");
                layer.Print(5, $"State: {_displayActor.State}");
            }

            if (_showTile)
            {
                // console.Print(1, 8, $"Move cost: {_displayTile.MoveCost.ToString()}");
                layer.Print(8, $"Occupied: {_displayTile.IsOccupied}");
                layer.Print(9, $"Walkable: {_displayTile.IsWalkable}");
                layer.Print(10, $"Wall: {_displayTile.IsWall}");
                layer.Print(11, $"Position: ({_displayTile.X}, {_displayTile.Y})");
            }
        }
    }
}
