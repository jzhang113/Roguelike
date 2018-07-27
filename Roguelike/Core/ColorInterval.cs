using RLNET;
using Pcg;
using System;
using System.Runtime.Serialization;

namespace Roguelike.Core
{
    [Serializable]
    struct ColorInterval : ISerializable
    {
        public RLColor Primary => _primary;

        private readonly RLColor _primary;
        private readonly RLColor _secondary;
        private readonly double _alpha;

        public ColorInterval(RLColor primary, RLColor secondary, double alpha)
        {
            _primary = primary;
            _secondary = secondary;
            _alpha = alpha;
        }

        private ColorInterval(SerializationInfo info, StreamingContext context)
        {
            float primaryR = (float)info.GetValue($"{nameof(_primary)}.r", typeof(float));
            float primaryG = (float)info.GetValue($"{nameof(_primary)}.g", typeof(float));
            float primaryB = (float)info.GetValue($"{nameof(_primary)}.b", typeof(float));
            _primary = new RLColor(primaryR, primaryG, primaryB);

            float secondaryR = (float)info.GetValue($"{nameof(_secondary)}.r", typeof(float));
            float secondaryG = (float)info.GetValue($"{nameof(_secondary)}.g", typeof(float));
            float secondaryB = (float)info.GetValue($"{nameof(_secondary)}.b", typeof(float));
            _secondary = new RLColor(secondaryR, secondaryG, secondaryB);

            _alpha = info.GetDouble(nameof(_alpha));
        }

        public RLColor GetColor(PcgRandom random) =>
            RLColor.Blend(_primary, _secondary, (float)(random.NextDouble() * _alpha));

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue($"{nameof(_primary)}.r", _primary.r);
            info.AddValue($"{nameof(_primary)}.g", _primary.g);
            info.AddValue($"{nameof(_primary)}.b", _primary.b);

            info.AddValue($"{nameof(_secondary)}.r", _secondary.r);
            info.AddValue($"{nameof(_secondary)}.g", _secondary.g);
            info.AddValue($"{nameof(_secondary)}.b", _secondary.b);

            info.AddValue(nameof(_alpha), _alpha);
        }
    }
}
