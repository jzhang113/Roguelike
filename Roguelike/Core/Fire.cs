﻿using Roguelike.Actions;
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
        public Drawable DrawingComponent { get; }
        public int X { get; set; }
        public int Y { get; set; }

        public string Name => "Fire";
        public int Energy { get; set; }
        public int ActivationEnergy => 0;
        public int Lifetime => -1; // Fires extinguish when out of fuel, not automatically

        public Fire(int x, int y)
        {
            ColorInterval foreground = new ColorInterval(Colors.Fire, Colors.FireAccent, 0.6);
            DrawingComponent = new AnimatedDrawable(foreground, null, '^');
            X = x;
            Y = y;
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

            return Game.Map.GetActor(X, Y).Match<ICommand>(
                some: _ => new ActionCommand(this,
                   new IgniteAction(new TargetZone(TargetShape.Self)),
                   new Loc(X, Y)),
                none: () => new WaitCommand(this));
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

            map.GetStack(X, Y).MatchSome(stack =>
            {
                stack.SetFire();
                map.RemoveStackIfEmpty(X, Y);
            });

            map.ComputeFov(X, Y, Constants.LIGHT_DECAY, false);
        }

        public int CompareTo(ISchedulable other)
        {
            return Energy - other.Energy;
        }
    }
}
