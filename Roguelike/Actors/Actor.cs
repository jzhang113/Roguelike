using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Skills;
using Roguelike.Systems;
using System.Collections.Generic;

namespace Roguelike.Actors
{
    public class Actor : Drawable, IActor, ISchedulable
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

        public override RLColor Color { get; set; }
        public override char Symbol { get; set; }
        public override int X { get; set; }
        public override int Y { get; set; }

        public int Energy { get; set; }
        public int RefreshRate { get; set; }

        public Weapon Weapon { get; set; }
        public Armor Armor { get; set; }
        public InventoryHandler Inventory { get; set; }

        public Actor()
        {
            Energy = 100;
            RefreshRate = 100;

            Inventory = new InventoryHandler();
            // TODO 1: Basic attacks should scale with stats.
            BasicAttack = new DamageSkill(100, 100);
        }

        public virtual bool IsDead() => HP < 0;
        public virtual void TriggerDeath() => Game.Map.RemoveActor(this);
        public virtual IAction Act() => SimpleAI.GetAction(this);

        public virtual int TakeDamage(int power)
        {
            HP -= power;
            return power;
        }

        public int CompareTo(ISchedulable other) => Energy - other.Energy;
    }
}
