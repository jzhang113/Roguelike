using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Interfaces;
using System;
using Roguelike.Data;
using Roguelike.World;
using Roguelike.Systems;

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
            ColorInterval foreground = new ColorInterval(new RLNET.RLColor(255, 185, 0), Swatch.DbBlood, 0.6);
            ColorInterval background = new ColorInterval(new RLNET.RLColor(200, 185, 0),
                new RLNET.RLColor(255, 185, 0), 0.6);

            DrawingComponent = new AnimatedDrawable(foreground, null, '^')
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

            (int dx, int dy) = Direction.DirectionList[Game.Random.Next(8)];
            Game.Map.SetFire(X + dx, Y + dy);

            ProcessFire(Game.Map);

            if (Game.Map.TryGetActor(X, Y, out _))
                return new ActionCommand(this,
                    new DamageAction(Constants.FIRE_DAMAGE, new TargetZone(TargetShape.Self)),
                    Game.Map.Field[X, Y]);

            return new WaitCommand(this);
        }

        // Flammable objects have a chance to get set on fire. Fire also produces light.
        private void ProcessFire(MapHandler map)
        {
            if (map.TryGetDoor(X, Y, out Door door))
            {
                // TODO: create an actor burning implementation
                // Q: do doors behave like items or actors when burned?
                MaterialProperty material = door.Parameters.Material.ToProperty();
                if (Game.Random.NextDouble() < material.Flammability.ToIgniteChance())
                    door.Open();
            }

            if (map.TryGetStack(X, Y, out InventoryHandler stack))
            {
                stack.SetFire();
                map.RemoveStackIfEmpty(X, Y);
            }

            foreach (var dir in Direction.DirectionList)
            {
                map.ComputeFovInOctant(X, Y, 0.1, dir, false);
            }
        }

        public int CompareTo(ISchedulable other)
        {
            return Energy - other.Energy;
        }
    }
}
