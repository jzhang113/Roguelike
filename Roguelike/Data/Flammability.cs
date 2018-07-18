using System;

namespace Roguelike.Data
{
    public enum Flammability
    {
        None,
        Low,
        Medium,
        High
    }

    static class FlammabilityExtensions
    {
        public static double ToIgniteChance(this Flammability flammability)
        {
            switch (flammability)
            {
                case Flammability.None:
                    return 0;
                case Flammability.Low:
                    return Constants.LOW_BURN_PERCENT;
                case Flammability.Medium:
                    return Constants.MEDIUM_BURN_PERCENT;
                case Flammability.High:
                    return Constants.HIGH_BURN_PERCENT;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flammability));
            }
        }
    }
}
