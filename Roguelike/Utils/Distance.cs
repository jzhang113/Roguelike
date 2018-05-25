namespace Roguelike.Utils
{
    // Helper methods for calculating distances
    static class Distance
    {
        public static int EuclideanDistanceSquared(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - y1;
            int dy = x2 - y2;

            return dx * dx + dy * dy;
        }
    }
}
