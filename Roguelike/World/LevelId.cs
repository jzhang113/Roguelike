using System;
using System.Collections.Generic;

namespace Roguelike.World
{
    [Serializable]
    public struct LevelId
    {
        public string RegionName { get; set; }
        public int Depth { get; set; }

        #region equality
        public override bool Equals(object obj)
        {
            if (!(obj is LevelId))
            {
                return false;
            }

            var level = (LevelId)obj;
            return RegionName == level.RegionName &&
                   Depth == level.Depth;
        }

        public override int GetHashCode()
        {
            var hashCode = 138455800;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(RegionName);
            hashCode = hashCode * -1521134295 + Depth.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(LevelId l1, LevelId l2)
        {
            return l1.Equals(l2);
        }

        public static bool operator !=(LevelId l1, LevelId l2)
        {
            return !l1.Equals(l2);
        }
        #endregion
    }
}
