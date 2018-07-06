using Roguelike.Actors;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Items;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    class EnchantAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed { get; } = Utils.Constants.FULL_TURN;
        public IAnimation Animation { get; } = null;

        IAnimation IAction.Animation => throw new NotImplementedException();

        public EnchantAction(TargetZone area)
        {
            Area = area;
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
