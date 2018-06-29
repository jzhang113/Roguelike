using System.Collections.Generic;

namespace Roguelike.World
{
    struct WorldParameter
    {
        public ICollection<Region> Regions { get; set; }
        public ConnectionConstraint Constraints { get; set; }
        public LevelId StartLevel { get; set; }

        internal struct Region
        {
            public string Name { get; set; }
            public int MinLength { get; set; }
            public int MaxLength { get; set; }
            public bool Require { get; set; }
            public bool Unique { get; set; }
        }

        internal struct ConnectionConstraint
        {
            public ICollection<LevelConnection> Require { get; set; }
            public ICollection<RegionConnection> Avoid { get; set; }
        }

        internal struct RegionConnection
        {
            public Region From { get; set; }
            public Region To { get; set; }
        }

        internal struct LevelConnection
        {
            public LevelId From { get; set; }
            public LevelId To { get; set; }
        }
    }
}
