namespace Roguelike.Interfaces
{
    interface IActor
    {
        string Name { get; set; }
        int Awareness { get; set; }
        int Speed { get; set; }

        int HP { get; set; }
        int SP { get; set; }
        int MP { get; set; }

        IAction BasicAttack { get; set; }

        void TakeDamage(int damage);
        bool IsDead();
        void TriggerDeath();
    }
}
