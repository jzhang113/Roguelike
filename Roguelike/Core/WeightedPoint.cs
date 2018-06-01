namespace Roguelike.Core
{
    public struct WeightedPoint
    {
        public int X { get; }
        public int Y { get; }
        public float Weight { get; }

        public WeightedPoint(int x, int y)
        {
            X = x;
            Y = y;
            Weight = 0;
        }

        public WeightedPoint(int x, int y, float weight)
        {
            X = x;
            Y = y;
            Weight = weight;
        }
    }
}
