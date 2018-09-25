using Pcg;
using Roguelike.Utils;
using System;
using System.Diagnostics.Contracts;
using System.Drawing;

namespace Roguelike.Core
{
    [Serializable]
    internal readonly struct ColorInterval
    {
        public Color Primary { get; }
        private Color Secondary { get; }
        private double Alpha { get; }

        public ColorInterval(Color primary, Color secondary, double alpha)
        {
            Primary = primary;
            Secondary = secondary;
            Alpha = alpha;
        }

        [Pure]
        public Color GetColor(PcgRandom random) =>
            Primary.Blend(Secondary, random.NextDouble() * Alpha);
    }
}
