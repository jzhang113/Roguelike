using Roguelike.Core;
using Roguelike.Data;
using System;
using System.Collections.Generic;

namespace Roguelike.Actors
{
    [Serializable]
    public class ActorParameters
    {
        public string Type { get; }
        public ICollection<ActorFlag> Flags { get; }

        public MaterialType Material { get; set; }

        public int Str { get; set; }
        public int Dex { get; set; }
        public int Int { get; set; }

        public int MaxHp { get; set; }
        public int MaxSp { get; set; }

        public int Awareness { get; set; }
        public int Speed { get; set; }

        public ActorParameters(string name, params ActorFlag[] flags)
        {
            Type = name;
            Flags = flags;
        }
    }
}