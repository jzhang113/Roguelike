using System;

namespace Roguelike.World
{
    [Serializable]
    public readonly struct LevelId
    {
        public string Name { get; }
        public RegionType RegionType { get; }
        public int Depth { get; }

        public LevelId(string name, RegionType type, int depth)
        {
            Name = name;
            RegionType = type;
            Depth = depth;
        }

        public override string ToString()
        {
            return $"{Name} ({RegionType} {Depth})";
        }

        #region equality
        public override bool Equals(object obj)
        {
            if (!(obj is LevelId))
            {
                return false;
            }

            var level = (LevelId)obj;
            return Name == level.Name &&
                   RegionType == level.RegionType &&
                   Depth == level.Depth;
        }

        public override int GetHashCode()
        {
            int hashCode = 138455800;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + Name.GetHashCode();
            hashCode = hashCode * -1521134295 + RegionType.GetHashCode();
            return hashCode * -1521134295 + Depth.GetHashCode();
        }

        public static bool operator ==(LevelId l1, LevelId l2) =>
            l1.Name == l2.Name &&
            l1.RegionType == l2.RegionType &&
            l1.Depth == l2.Depth;

        public static bool operator !=(LevelId l1, LevelId l2) => !(l1 == l2);
        #endregion
    }
}
