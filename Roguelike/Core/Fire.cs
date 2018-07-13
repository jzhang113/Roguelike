using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Interfaces;
using Roguelike.Items;
using Roguelike.Systems;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Fire : ISchedulable
    {
        public int Fuel { get; private set; }

        public Drawable DrawingComponent { get; }
        public int X { get => DrawingComponent.X; set => DrawingComponent.X = value; }
        public int Y { get => DrawingComponent.Y; set => DrawingComponent.Y = value; }

        public string Name => "fire";
        public int Energy { get; set; }
        public int RefreshRate => Utils.Constants.DEFAULT_REFRESH_RATE;

        public Fire(int x, int y)
        {
            DrawingComponent = new Drawable()
            {
                Color = Swatch.DbBlood,
                Symbol = '^',
                X = x,
                Y = y
            };

            Fuel = 10;
        }

        public ICommand Act()
        {
            if (--Fuel <= 0)
                Game.Map.RemoveFire(this);

            if (Game.World.CombatRandom.Next(10) > 1)
            {
                WeightedPoint dir = Direction.Directions[Game.World.CombatRandom.Next() % 8];
                Game.Map.SetFire(X + dir.X, Y + dir.Y);
            }

            //if (Game.Map.TryGetDoor(X, Y, out Door door))
            //{
            //    // TODO: wooden doors burn
            //}

            //if (Game.Map.TryGetStack(X, Y, out InventoryHandler stack))
            //{
            //    // TODO: items catch fire
            //    // stack.SetFire();
            //}

            if (Game.Map.TryGetActor(X, Y, out _))
                return new ActionCommand(this,
                    new DamageAction(Utils.Constants.FIRE_DAMAGE, new TargetZone(Enums.TargetShape.Self)),
                    Game.Map.Field[X, Y]);

            return new WaitCommand(this);
        }

        public int CompareTo(ISchedulable other)
        {
            return Energy - other.Energy;
        }
    }
}
