using System;
using Roguelike.Actors;
using Roguelike.Core;
using Roguelike.Items;

namespace Roguelike.Actions
{
    [Serializable]
    class EnchantAction : IAction
    {
        public int Speed { get; }
        public TargetZone Area { get; }

        public EnchantAction(TargetZone area)
        {
            Area = area;
            Speed = Utils.Constants.FULL_TURN;
        }

        public void Activate(Actor source, Terrain target)
        {
            if (target == null)
                return;

            if (!Game.Map.TryGetItem(target.X, target.Y, out ItemCount itemCount))
                return;

            itemCount.Item.Enchantment++;
        }
    }
}
