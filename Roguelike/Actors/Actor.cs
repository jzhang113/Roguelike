using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System;

namespace Roguelike.Actors
{
    [Serializable]
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

        public int STR { get; }
        public int DEX { get; }
        public int DEF { get; set; }
        public int INT { get; set; }
        
        public Enums.ActorState State { get; set; }

        public override int X { get; set; }
        public override int Y { get; set; }

        public int Energy { get; set; }
        public int RefreshRate { get; set; }

        public InventoryHandler Inventory { get; }
        public EquipmentHandler Equipment { get; }

        public bool IsDead { get => HP < 0; }

        public Actor()
        {
            RefreshRate = Utils.Constants.DEFAULT_REFRESH_RATE;
            Inventory = new InventoryHandler();

            Weapon defaultWeapon = new Weapon("fists", Materials.Flesh);
            Equipment = new EquipmentHandler(defaultWeapon);
        }

        public virtual void TriggerDeath()
        {
            Game.Map.RemoveActor(this);
            Game.EventScheduler.RemoveActor(this);
        }

        public virtual ICommand Act()
        {
            System.Diagnostics.Debug.Assert(Game.GameMode != Enums.Mode.Targetting);
            return SimpleAI.GetAction(this);
        }

        public int TakeDamage(int power)
        {
            HP -= power;
            return power;
        }

        public int TakeHealing(int power)
        {
            int restore = MaxHP - HP;
            restore = (restore > 0) ? restore : 0;

            if (restore > power)
            {
                HP += power;
                return power;
            }
            else
            {
                HP += restore;
                return restore;
            }
        }

        // Returns the Schedulable with higher energy. If there is a tie with another Actor, the
        // Actor closer to the Player gets to move first to preserve proper pathing. However, 
        // since the Player is also an Actor, this does not apply to cases when other is Player.
        public int CompareTo(ISchedulable other)
        {
            int energyDiff = Energy - other.Energy;

            if (energyDiff == 0 && other is Actor) //&& !(other is Player)) Q: do we need this??
            {
                Actor otherActor = other as Actor;
                return (int)(Game.Map.PlayerMap[X, Y] - Game.Map.PlayerMap[otherActor.X, otherActor.Y]);
            }

            return energyDiff;
        }
    }
}
