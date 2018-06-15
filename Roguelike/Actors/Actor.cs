using RLNET;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Actors
{
    [Serializable]
    public class Actor : ISchedulable, ISerializable
    {
        public string Name { get; set; }
        public int Awareness { get; set; }
        public int Speed { get; set; }
        public bool BlocksLight { get; set; }

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

        public int Energy { get; set; }
        public int RefreshRate { get; set; }

        public InventoryHandler Inventory { get; }
        public EquipmentHandler Equipment { get; }
        public Drawable DrawingComponent { get; }

        public int X
        {
            get => DrawingComponent.X;
            set => DrawingComponent.X = value;
        }

        public int Y
        {
            get => DrawingComponent.Y;
            set => DrawingComponent.Y = value;
        }

        public bool IsDead => HP < 0;

        public Actor()
        {
            Energy = 0;
            RefreshRate = Utils.Constants.DEFAULT_REFRESH_RATE;
            Inventory = new InventoryHandler();

            Weapon defaultWeapon = new Weapon("fists", Materials.Flesh);
            Equipment = new EquipmentHandler(defaultWeapon);

            DrawingComponent = new Drawable();

            BlocksLight = true;
        }

        protected Actor(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString(nameof(Name));
            Awareness = info.GetInt32(nameof(Awareness));
            Speed = info.GetInt32(nameof(Speed));
            BlocksLight = info.GetBoolean(nameof(BlocksLight));

            HP = info.GetInt32(nameof(HP));
            MaxHP = info.GetInt32(nameof(MaxHP));
            SP = info.GetInt32(nameof(SP));
            MaxSP = info.GetInt32(nameof(MaxSP));
            MP = info.GetInt32(nameof(MP));
            MaxMP = info.GetInt32(nameof(MaxMP));
            
            STR = info.GetInt32(nameof(STR));
            DEX = info.GetInt32(nameof(DEX));
            DEF = info.GetInt32(nameof(DEF));
            INT = info.GetInt32(nameof(INT));
            
            State = (Enums.ActorState)info.GetValue(nameof(State), typeof(Enums.ActorState));
            
            Energy = info.GetInt32(nameof(Energy));
            RefreshRate = info.GetInt32(nameof(RefreshRate));
            
            Inventory = (InventoryHandler)info.GetValue(nameof(Inventory), typeof(InventoryHandler));
            Equipment = (EquipmentHandler)info.GetValue(nameof(Equipment), typeof(EquipmentHandler));
            DrawingComponent = (Drawable)info.GetValue(nameof(DrawingComponent), typeof(Drawable));
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

            if (energyDiff == 0 && other is Actor)
            {
                Actor otherActor = other as Actor;
                float distPlayer = Game.Map.PlayerMap[X, Y];
                float otherDistPlayer = Game.Map.PlayerMap[otherActor.X, otherActor.Y];

                // if we haven't discovered one of the actors, don't change the order
                if (float.IsNaN(distPlayer) || float.IsNaN(otherDistPlayer))
                    return 0;
                else
                    return (int)(otherDistPlayer - distPlayer);
            }

            return energyDiff;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Awareness), Awareness);
            info.AddValue(nameof(Speed), Speed);
            info.AddValue(nameof(BlocksLight), BlocksLight);

            info.AddValue(nameof(HP), HP);
            info.AddValue(nameof(MaxHP), MaxHP);
            info.AddValue(nameof(SP), SP);
            info.AddValue(nameof(MaxSP), MaxSP);
            info.AddValue(nameof(MP), MP);
            info.AddValue(nameof(MaxMP), MaxMP);

            info.AddValue(nameof(STR), STR);
            info.AddValue(nameof(DEX), DEX);
            info.AddValue(nameof(DEF), DEF);
            info.AddValue(nameof(INT), INT);

            info.AddValue(nameof(State), State);

            info.AddValue(nameof(Energy), Energy);
            info.AddValue(nameof(RefreshRate), RefreshRate);

            info.AddValue(nameof(Inventory), Inventory);
            info.AddValue(nameof(Equipment), Equipment);
            info.AddValue(nameof(DrawingComponent), DrawingComponent);
        }
    }
}
