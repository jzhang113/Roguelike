namespace Roguelike.Systems
{
    class WeightedPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public float Weight { get; set; }

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
