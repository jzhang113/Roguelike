using MessagePack;
using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Statuses;
using Roguelike.Systems;

namespace Roguelike.Actors
{
    [MessagePackObject]
    public class Actor : ISchedulable
    {
        [IgnoreMember]
        public string Name => Parameters.Type;
        [Key(0)]
        public bool BlocksLight { get; set; }

        [Key(1)]
        public int Hp { get; set; }
        [Key(2)]
        public int Mp { get; set; }
        [Key(3)]
        public int Sp { get; set; }
        [Key(4)]
        public int Armor { get; set; }

        [Key(5)]
        public ActorState State { get; set; }

        [Key(6)]
        public int Energy { get; set; }
        [Key(7)]
        public int RefreshRate { get; set; }

        [Key(8)]
        public InventoryHandler Inventory { get; }
        [Key(9)]
        public Drawable DrawingComponent { get; }
        [Key(10)]
        public ActorParameters Parameters { get; }
        [Key(11)]
        public StatusHandler StatusHandler { get; }

        [Key(12)]
        public int X { get; set; }
        [Key(13)]
        public int Y { get; set; }

        [IgnoreMember]
        public virtual IAction BasicAttack => new DamageAction(5, new TargetZone(TargetShape.Directional));

        // Deserialization constructor
        public Actor()
        {
        }

        public Actor(ActorParameters parameters, RLNET.RLColor color, char symbol)
        {
            Parameters = parameters;
            Hp = Parameters.MaxHp;
            Sp = Parameters.MaxSp;
            Mp = Parameters.MaxMp;
            // TODO: calculate armor from equipment
            Armor = 30;

            Energy = 0;
            RefreshRate = Data.Constants.DEFAULT_REFRESH_RATE;
            Inventory = new InventoryHandler();
            StatusHandler = new StatusHandler();
            StatusHandler.AddStatus(StatusType.Phasing, 10);

            DrawingComponent = new Drawable(color, symbol, false);
            BlocksLight = true;
        }

        public virtual void TriggerDeath()
        {
            Game.Map.RemoveActor(this);
            Game.EventScheduler.RemoveActor(this);

            if (Game.Map.Field[X, Y].IsVisible)
            {
                Game.MessageHandler.AddMessage($"{Name} dies");
                Game.Map.Refresh();
            }
        }

        public ICommand Act()
        {
            ICommand command = GetAction();
            if (command == null)
                return null;

            StatusHandler.Process();
            if (IsDead())
                TriggerDeath();

            return command;
        }

        public bool IsDead() => Hp < 0;

        public virtual ICommand GetAction() => SimpleAI.GetAction(this);

        public int TakeDamage(int power)
        {
            int blocked = power * Armor / 30;
            int damage = power - blocked;
            Armor -= blocked;
            Hp -= damage;
            return damage;
        }
        public int TakeHealing(int power)
        {
            int restore = Parameters.MaxHp - Hp;
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
    }
}
