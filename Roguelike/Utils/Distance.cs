namespace Roguelike.Utils
{
    // Helper methods for calculating distances
    static class Distance
    {
        public static double EuclideanDistance(int x1, int y1, int x2, int y2)
        {
            return System.Math.Sqrt(EuclideanDistanceSquared(x1, y1, x2, y2));
        }

        public static int EuclideanDistanceSquared(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2;
            int dy = y1 - y2;

            return dx * dx + dy * dy;
        }
    }
}
