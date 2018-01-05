namespace Roguelike.Interfaces
{
    interface IActor
    {
        string Name { get; set; }
        int Awareness { get; set; }
        int Speed { get; set; }
        int X { get; set; }
        int Y { get; set; }

        int HP { get; set; }
        int MaxHP { get; set; }
        int SP { get; set; }
        int MaxSP { get; set; }
        int MP { get; set; }
        int MaxMP { get; set; }

        int STR { get; set; }
        int DEX { get; set; }
        int DEF { get; set; }
        int INT { get; set; }

        ISkill BasicAttack { get; set; }
        ActorState State { get; set; }

        int TakeDamage(int damage);
        bool IsDead();
        void TriggerDeath();
    }

    enum ActorState { Wander, Sleep, Chase, Flee, Dead };
}
