using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Interfaces;
using System;
using Roguelike.Data;

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
        public int RefreshRate => Constants.DEFAULT_REFRESH_RATE;

        public Fire(int x, int y)
        {
            DrawingComponent = new AnimatedDrawable(new RLNET.RLColor(255, 185, 0), '^', Swatch.DbBlood, 0.6)
            {
                X = x,
                Y = y
            };

            Fuel = 10;
        }

        public ICommand Act()
        {
            Tile tile = Game.Map.Field[X, Y];
            if (--tile.Fuel <= 0)
            {
                Game.Map.RemoveFire(this);
                tile.Type = TerrainType.Stone; // Q: does everything burn into stone?
                return new WaitCommand(this);
            }

            WeightedPoint dir = Direction.Directions[Game.World.Random.Next(8)];
            Game.Map.SetFire(X + dir.X, Y + dir.Y);

            Game.Map.ProcessFire(this);

            if (Game.Map.TryGetActor(X, Y, out _))
                return new ActionCommand(this,
                    new DamageAction(Constants.FIRE_DAMAGE, new TargetZone(TargetShape.Self)),
                    Game.Map.Field[X, Y]);

            return new WaitCommand(this);
        }

        public int CompareTo(ISchedulable other)
        {
            return Energy - other.Energy;
        }
    }
}
