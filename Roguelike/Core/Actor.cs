using RLNET;
using Roguelike.Interfaces;
using RogueSharp;

namespace Roguelike.Core
{
    class Actor : IActor, ISchedulable, IDrawable
    {
        public string Name { get; set; }
        public int Awareness { get; set; }
        public int Speed { get; set; }

        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int SP { get; set; }
        public int MaxSP { get; set; }
        public int MP { get; set; }
        public int MaxMP { get; set; }

        public int STR { get; set; }
        public int DEX { get; set; }
        public int DEF { get; set; }
        public int INT { get; set; }

        public ISkill BasicAttack { get; set; }
        public ActorState State { get; set; }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public int Energy { get; set; } = 100;
        public int RefreshRate { get; set; } = 100;

        public virtual bool IsDead() => HP < 0;
        public virtual void TriggerDeath() => Game.Map.RemoveActor(this);
        public virtual IAction Act() => SimpleAI.GetAction(this) ?? null;

        public virtual int TakeDamage(int power)
        {
            HP -= power;
            return power;
        }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
                return;

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                //console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
        }

        public int CompareTo(ISchedulable other) => Energy - other.Energy;
    }
}
