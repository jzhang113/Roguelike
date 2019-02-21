using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Data;
using Roguelike.Interfaces;
using Roguelike.World;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Fire : ISchedulable
    {
        public Drawable DrawingComponent { get; }
        public Loc Loc { get; set; }

        public string Name => "Fire";
        public int Energy { get; set; }
        public int ActivationEnergy => 0;
        public int Lifetime => -1; // Fires extinguish when out of fuel, not automatically

        public Fire(Loc loc)
        {
            ColorInterval foreground = new ColorInterval(Colors.Fire, Colors.FireAccent, 0.6);
            DrawingComponent = new AnimatedDrawable(foreground, null, '^');
            Loc = loc;
        }

        public ICommand Act()
        {
            Tile tile = Game.Map.Field[Loc];
            if (--tile.Fuel <= 0)
            {
                Game.Map.RemoveFire(this);
                tile.Type = TerrainType.Stone; // Q: does everything burn into stone?
                return new WaitCommand(this);
            }

            (int dx, int dy) = Direction.DirectionList[Game.Random.Next(8)];
            Game.Map.SetFire(Loc + (dx, dy));

            ProcessFire(Game.Map);

            return Game.Map.GetActor(Loc).Match<ICommand>(
                some: _ => new ActionCommand(this,
                   new IgniteAction(new TargetZone(TargetShape.Self)),
                   Loc),
                none: () => new WaitCommand(this));
        }

        // Flammable objects have a chance to get set on fire. Fire also produces light.
        private void ProcessFire(MapHandler map)
        {
            if (map.TryGetDoor(Loc, out Door door))
            {
                // TODO: create an actor burning implementation
                // Q: do doors behave like items or actors when burned?
                MaterialProperty material = door.Parameters.Material.ToProperty();
                if (Game.Random.NextDouble() < material.Flammability.ToIgniteChance())
                    door.Open();
            }

            map.GetStack(Loc).MatchSome(stack =>
            {
                stack.SetFire();
                map.RemoveStackIfEmpty(Loc);
            });

            map.ComputeFov(Loc, Constants.LIGHT_DECAY, false);
        }

        public int CompareTo(ISchedulable other)
        {
            return Energy - other.Energy;
        }
    }
}
