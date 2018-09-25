using System.Collections.Generic;

namespace Roguelike.World
{
    public enum RegionType
    {
        Root,
        Main,
        Side,
        Otherside
    }

    public struct WorldParameter
    {
        public ICollection<RegionData> Regions { get; set; }
        public int MinWorldSize { get; set; }
        public int MaxWorldSize { get; set; }

        public struct RegionData
        {
            public RegionType Type { get; set; }
            public int MinLength { get; set; }
            public int MaxLength { get; set; }
            public bool Require { get; set; }
            public bool Unique { get; set; }
            public ConnectionConstraint Constraints { get; set; }
        }

        public struct ConnectionConstraint
        {
            public ICollection<RequireId> Require { get; set; }
            public ICollection<RegionType> Avoid { get; set; }
        }

        public struct RegionConnection
        {
            public RegionData From { get; set; }
            public RegionData To { get; set; }
        }

        public struct RequireId
        {
            public RegionType RegionType { get; set; }
            public int MinDepth { get; set; }
        }
    }
}
