using System;

namespace Roguelike.Actors
{
    [Serializable]
    public class ActorParameters
    {
        public string Name { get; }

        public int Str { get; set; }
        public int Dex { get; set; }
        public int Int { get; set; }

        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int MaxSp { get; set; }

        public int Awareness { get; set; }
        public int Speed { get; set; }

        public ActorParameters(string name)
        {
            Name = name;
        }
    }
}