using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace Roguelike.Core
{
    struct Direction
    {
        public static WeightedPoint N { get; } = new WeightedPoint(0, -1, 1);
        public static WeightedPoint E { get; } = new WeightedPoint(1, 0, 1);
        public static WeightedPoint S { get; } = new WeightedPoint(0, 1, 1);
        public static WeightedPoint W { get; } = new WeightedPoint(-1, 0, 1);
        public static WeightedPoint NE { get; } = new WeightedPoint(1, -1, 1.5f);
        public static WeightedPoint SE { get; } = new WeightedPoint(1, 1, 1.5f);
        public static WeightedPoint SW { get; } = new WeightedPoint(-1, 1, 1.5f);
        public static WeightedPoint NW { get; } = new WeightedPoint(-1, -1, 1.5f);

        public static readonly IList<WeightedPoint> Directions = new[]{ N, E, S, W, NE, SE, SW, NW };
    }
}
