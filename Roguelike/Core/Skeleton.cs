namespace Roguelike.Core
{
    class Skeleton : Actor
    {
        public Skeleton()
        {
            Awareness = 10;
            Name = "Skeleton";
            Color = Colors.TextHeading;
            Symbol = 'S';

            HP = 50;
            SP = 20;
            MP = 20;
            BasicAttack = new BumpAttack(10);
        }
    }
}
