using System.Collections.Generic;
using Roguelike.Interfaces;
using Roguelike.Systems;

namespace Roguelike.Core
{
    class Move
    {
        public static WeightedPoint N { get; } = new WeightedPoint(0, -1, 1);
        public static WeightedPoint E { get; } = new WeightedPoint(1, 0, 1);
        public static WeightedPoint S { get; } = new WeightedPoint(0, 1, 1);
        public static WeightedPoint W { get; } = new WeightedPoint(-1, 0, 1);
        public static WeightedPoint NE { get; } = new WeightedPoint(1, -1, 1.4f);
        public static WeightedPoint SE { get; } = new WeightedPoint(1, 1, 1.4f);
        public static WeightedPoint SW { get; } = new WeightedPoint(-1, 1, 1.4f);
        public static WeightedPoint NW { get; } = new WeightedPoint(-1, -1, 1.4f);

        public static IEnumerable<WeightedPoint> Directions = new WeightedPoint[]{ N, NE, E, SE, S, SW, W, NW };

        public static ICommand MoveN { get; } = new MoveCommand(0, -1);
        public static ICommand MoveNE { get; } = new MoveCommand(1, -1);
        public static ICommand MoveE { get; } = new MoveCommand(1, 0);
        public static ICommand MoveSE { get; } = new MoveCommand(1, 1);
        public static ICommand MoveS { get; } = new MoveCommand(0, 1);
        public static ICommand MoveSW { get; } = new MoveCommand(-1, 1);
        public static ICommand MoveW { get; } = new MoveCommand(-1, 0);
        public static ICommand MoveNW { get; } = new MoveCommand(-1, -1);
        
    }
}
