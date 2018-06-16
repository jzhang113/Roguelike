using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System;
using System.Runtime.Serialization;
using Roguelike.Commands;

namespace Roguelike.Actors
{
    [Serializable]
    public class Actor : ISchedulable, ISerializable
    {
        public string Name { get; set; }
        public int Awareness { get; set; }
        public int Speed { get; set; }
        public bool BlocksLight { get; set; }

        public int Hp { get; set; }
        public int MaxHp { get; set; }
        public int Sp { get; set; }
        public int MaxSp { get; set; }
        public int Mp { get; set; }
        public int MaxMp { get; set; }

        public int Str { get; }
        public int Dex { get; }
        public int Def { get; set; }
        public int Int { get; set; }

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

        public bool IsDead => Hp < 0;

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

            Hp = info.GetInt32(nameof(Hp));
            MaxHp = info.GetInt32(nameof(MaxHp));
            Sp = info.GetInt32(nameof(Sp));
            MaxSp = info.GetInt32(nameof(MaxSp));
            Mp = info.GetInt32(nameof(Mp));
            MaxMp = info.GetInt32(nameof(MaxMp));
            
            Str = info.GetInt32(nameof(Str));
            Dex = info.GetInt32(nameof(Dex));
            Def = info.GetInt32(nameof(Def));
            Int = info.GetInt32(nameof(Int));
            
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
            Hp -= power;
            return power;
        }

        public int TakeHealing(int power)
        {
            int restore = MaxHp - Hp;
            restore = (restore > 0) ? restore : 0;

            if (restore > power)
            {
                Hp += power;
                return power;
            }
            else
            {
                Hp += restore;
                return restore;
            }
        }

        // Returns the Schedulable with higher energy. If there is a tie with another Actor, the
        // Actor closer to the Player gets to move first to preserve proper pathing. However, 
        // since the Player is also an Actor, this does not apply to cases when other is Player.
        public int CompareTo(ISchedulable other)
        {
            int energyDiff = Energy - other.Energy;

            if (energyDiff != 0 || !(other is Actor otherActor))
                return energyDiff;

            float distPlayer = Game.Map.PlayerMap[X, Y];
            float otherDistPlayer = Game.Map.PlayerMap[otherActor.X, otherActor.Y];

            // if we haven't discovered one of the actors, don't change the order
            return float.IsNaN(distPlayer) || float.IsNaN(otherDistPlayer)
                ? 0
                : (int) (otherDistPlayer - distPlayer);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Awareness), Awareness);
            info.AddValue(nameof(Speed), Speed);
            info.AddValue(nameof(BlocksLight), BlocksLight);

            info.AddValue(nameof(Hp), Hp);
            info.AddValue(nameof(MaxHp), MaxHp);
            info.AddValue(nameof(Sp), Sp);
            info.AddValue(nameof(MaxSp), MaxSp);
            info.AddValue(nameof(Mp), Mp);
            info.AddValue(nameof(MaxMp), MaxMp);

            info.AddValue(nameof(Str), Str);
            info.AddValue(nameof(Dex), Dex);
            info.AddValue(nameof(Def), Def);
            info.AddValue(nameof(Int), Int);

            info.AddValue(nameof(State), State);

            info.AddValue(nameof(Energy), Energy);
            info.AddValue(nameof(RefreshRate), RefreshRate);

            info.AddValue(nameof(Inventory), Inventory);
            info.AddValue(nameof(Equipment), Equipment);
            info.AddValue(nameof(DrawingComponent), DrawingComponent);
        }
    }
}
