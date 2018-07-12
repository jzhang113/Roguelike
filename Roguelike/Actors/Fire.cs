using Roguelike.Actions;
using Roguelike.Commands;
using Roguelike.Core;
using Roguelike.Items;
using Roguelike.Systems;
using System;

namespace Roguelike.Actors
{
    [Serializable]
    public class Fire : Actor
    {
        public int Fuel { get; private set; }

        public Fire(int x, int y) : base(new ActorParameters("fire"), Swatch.DbBlood, '^')
        {
            X = x;
            Y = y;
            BlocksLight = false;

            Fuel = 10;
        }

        public override ICommand Act()
        {
            Fuel--;
            if (Fuel <= 0)
                TriggerDeath();

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

            //if (Game.Map.TryGetActor(X, Y, out _))
            //    return new ActionCommand(this,
            //        new DamageAction(10, new TargetZone(Enums.TargetShape.Self)),
            //        Game.Map.Field[X, Y]);

            return new WaitCommand(this);
        }

        public override void TriggerDeath()
        {
            Game.Map.RemoveFire(this);
        }
    }
}
