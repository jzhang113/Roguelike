using RLNET;
using Roguelike.Core;

namespace Roguelike.Systems
{
    class LookHandler
    {
        private static Actor _displayActor;
        private static Terrain _displayTile;
        private static bool _display;

        public static void Display(Actor actor, Terrain tile)
        {
            _displayActor = actor;
            _displayTile = tile;
            _display = (actor != null);
        }

        public static void Draw(RLConsole console)
        {
            if (_display)
            {
                console.Print(1, 1, _displayActor.Name, Colors.TextHeading);
                console.Print(1, 2, "HP: " + _displayActor.HP + " / " + _displayActor.MaxHP, Colors.TextHeading);
                console.Print(1, 3, "MP: " + _displayActor.MP + " / " + _displayActor.MaxMP, Colors.TextHeading);
                console.Print(1, 4, "SP: " + _displayActor.SP + " / " + _displayActor.MaxSP, Colors.TextHeading);
                console.Print(1, 5, "Energy: " + _displayActor.Energy.ToString(), Colors.TextHeading);
                console.Print(1, 6, "State: " + _displayActor.State, Colors.TextHeading);
            }
            
            console.Print(1, 8, _displayTile.MoveCost.ToString(), Colors.TextHeading);
        }
    }
}
