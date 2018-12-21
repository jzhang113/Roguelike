using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using Roguelike.Items;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    internal class EnchantAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => Data.Constants.FULL_TURN;
        public IAnimation Animation => null;

        public EnchantAction(TargetZone area)
        {
            Area = area;
        }

        public void Activate(ISchedulable source, Tile target)
        {
            if (target == null)
                return;

            if (!Game.Map.TryGetItem(target.X, target.Y, out Item item))
                return;

            item.Enchantment++;
        }
    }
}
