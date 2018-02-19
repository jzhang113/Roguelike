using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;

namespace Roguelike.Actors
{
    public class Actor : Drawable, ISchedulable
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

        public Weapon DefaultWeapon { get; private set; }
        public bool IsDead { get => HP < 0; }

        public Actor()
        {
            Energy = 100;
            RefreshRate = 100;

            Inventory = new InventoryHandler();
            DefaultWeapon = new Fists(this);
            Weapon = DefaultWeapon;
        }

        public virtual void TriggerDeath()
        {
            Game.Map.RemoveActor(this);
            Game.EventScheduler.RemoveActor(this);
        }
        public virtual IAction Act() => SimpleAI.GetAction(this);

        public int TakeDamage(int power)
        {
            HP -= power;
            return power;
        }

        public int TakeHealing(int power)
        {
            int damage = MaxHP - HP;

            if (damage > power)
            {
                HP += power;
                return power;
            }
            else
            {
                HP += damage;
                return damage;
            }
        }

        // Returns the Schedulable with higher energy. If there is a tie with another Actor, the
        // Actor closer to the Player gets to move first to preserve proper pathing. However, 
        // since the Player is also an Actor, this does not apply to cases when other is Player.
        public int CompareTo(ISchedulable other)
        {
            int energyDiff = Energy - other.Energy;

            if (energyDiff == 0 && other is Actor && !(other is Player))
            {
                Actor otherActor = other as Actor;
                return (int)(Game.Map.PlayerMap[X, Y] - Game.Map.PlayerMap[otherActor.X, otherActor.Y]);
            }

            return energyDiff;
        }

        public int Distance2(Actor other)
        {
            int dx = X - other.X;
            int dy = Y - other.Y;

            return dx * dx + dy * dy;
        }
    }
}
