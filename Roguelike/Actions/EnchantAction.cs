using Optional;
using Roguelike.Animations;
using Roguelike.Core;
using Roguelike.Interfaces;
using System;

namespace Roguelike.Actions
{
    [Serializable]
    internal class EnchantAction : IAction
    {
        public TargetZone Area { get; }
        public int Speed => 0;
        public int EnergyCost => Data.Constants.FULL_TURN;
        public Option<IAnimation> Animation => Option.None<IAnimation>();

        public EnchantAction(TargetZone area)
        {
            Area = area;
        }

        public void Activate(ISchedulable source, Tile target) =>
            Game.Map.GetItem(target.X, target.Y).MatchSome(item => item.Enchantment++);
    }
}
