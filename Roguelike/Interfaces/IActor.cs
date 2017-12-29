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

        int STR { get; set; }
        int DEX { get; set; }
        int DEF { get; set; }
        int INT { get; set; }

        ISkill BasicAttack { get; set; }

        int TakeDamage(int damage);
        bool IsDead();
        void TriggerDeath();
    }
}
