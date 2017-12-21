namespace Roguelike.Core
{
    class Player : Actor
    {
        public Player()
        {
            Awareness = 15;
            Name = "Player";
            Color = Colors.Player;
            Symbol = '@';
            X = 10;
            Y = 10;
        }
    }
}
