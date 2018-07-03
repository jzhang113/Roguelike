using System.Collections.Generic;

namespace Roguelike.World
{
    struct WorldParameter
    {
        public ICollection<RegionData> Regions { get; set; }
        public int MinWorldSize { get; set; }
        public int MaxWorldSize { get; set; }

        internal struct RegionData
        {
            public RegionType Type { get; set; }
            public int MinLength { get; set; }
            public int MaxLength { get; set; }
            public bool Require { get; set; }
            public bool Unique { get; set; }
            public ConnectionConstraint Constraints { get; set; }
        }

        internal struct ConnectionConstraint
        {
            public ICollection<LevelId> Require { get; set; }
            public ICollection<RegionType> Avoid { get; set; }
        }

        internal struct RegionConnection
        {
            public RegionData From { get; set; }
            public RegionData To { get; set; }
        }

        internal struct LevelConnection
        {
            public LevelId From { get; set; }
            public LevelId To { get; set; }
        }
    }

    public enum RegionType
    {
        Root,
        Main,
        Side,
        Otherside
    }
}
