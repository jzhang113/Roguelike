using RLNET;
using Pcg;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Roguelike.Core
{
    [Serializable]
    internal readonly struct ColorInterval : ISerializable
    {
        public RLColor Primary { get; }
        private RLColor Secondary { get; }
        private double Alpha { get; }

        public ColorInterval(RLColor primary, RLColor secondary, double alpha)
        {
            Primary = primary;
            Secondary = secondary;
            Alpha = alpha;
        }

        private ColorInterval(SerializationInfo info, StreamingContext context)
        {
            float primaryR = (float)info.GetValue($"{nameof(Primary)}.r", typeof(float));
            float primaryG = (float)info.GetValue($"{nameof(Primary)}.g", typeof(float));
            float primaryB = (float)info.GetValue($"{nameof(Primary)}.b", typeof(float));
            Primary = new RLColor(primaryR, primaryG, primaryB);

            float secondaryR = (float)info.GetValue($"{nameof(Secondary)}.r", typeof(float));
            float secondaryG = (float)info.GetValue($"{nameof(Secondary)}.g", typeof(float));
            float secondaryB = (float)info.GetValue($"{nameof(Secondary)}.b", typeof(float));
            Secondary = new RLColor(secondaryR, secondaryG, secondaryB);

            Alpha = info.GetDouble(nameof(Alpha));
        }

        [Pure]
        public RLColor GetColor(PcgRandom random) =>
            RLColor.Blend(Primary, Secondary, (float)(random.NextDouble() * Alpha));

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(Primary)}.r", Primary.r);
            info.AddValue($"{nameof(Primary)}.g", Primary.g);
            info.AddValue($"{nameof(Primary)}.b", Primary.b);

            info.AddValue($"{nameof(Secondary)}.r", Secondary.r);
            info.AddValue($"{nameof(Secondary)}.g", Secondary.g);
            info.AddValue($"{nameof(Secondary)}.b", Secondary.b);

            info.AddValue(nameof(Alpha), Alpha);
        }
    }
}
