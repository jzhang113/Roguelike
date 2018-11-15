using System;
using System.Collections.Generic;

namespace Roguelike.World
{
    [Serializable]
    internal class LevelData
    {
        public bool Seen { get; set; }
        public ICollection<LevelId> Exits { get; set; }
        internal int Seed { get; set; }
    }
}