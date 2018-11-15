using Pcg;
using System;

namespace Roguelike.Utils
{
    public static class RandomExtensions
    {
        public static double NextNormal(this PcgRandom rand, double mean, double variance)
        {
            // Box-Muller transform
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mean + Math.Sqrt(variance) * randStdNormal;

            return randNormal;
        }

        public static int NextBinomial(this PcgRandom rand, int n, double p)
        {
            if (n <= 30)
            {
                // Run Bernouilli samples for small n.
                int successes = 0;
                for (int i = 0; i < n; i++)
                {
                    if (rand.NextDouble() < p)
                        successes++;
                }

                return successes;
            }
            else
            {
                // Approximate with the normal distribution for large n.
                int normal = (int)rand.NextNormal(n * p, n * p * (1 - p));
                if (normal < 0)
                    return 0;
                else if (normal > n)
                    return n;
                else
                    return normal;
            }
        }
    }
}
