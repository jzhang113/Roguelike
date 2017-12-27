using RLNET;
using Roguelike.Interfaces;
using RogueSharp;

namespace Roguelike.Core
{
    class Actor : IActor, IDrawable
    {
        public string Name { get; set; }
        public int Awareness { get; set; }
        public int Speed { get; set; }

        public int HP { get; set; }
        public int SP { get; set; }
        public int MP { get; set; }

        public IAction BasicAttack { get; set; }

        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public virtual void TakeDamage(int damage)
        {
            HP -= damage;
        }

        public virtual bool IsDead()
        {
            return (HP < 0) ? true : false;
        }

        public virtual void TriggerDeath()
        {
            Game.Map.RemoveActor(this);
        }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, Colors.Floor, Colors.FloorBackground, '.');
            }
        }
    }
}
