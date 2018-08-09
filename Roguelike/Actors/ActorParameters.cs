using Roguelike.Data;

namespace Roguelike.Actors
{
    public class ActorParameters
    {
        public string Type { get; }
        public MaterialType Material { get; set; }

        public int Str { get; set; }
        public int Dex { get; set; }
        public int Int { get; set; }

        public int MaxHp { get; set; }
        public int MaxMp { get; set; }
        public int MaxSp { get; set; }

        public int Awareness { get; set; }
        public int Speed { get; set; }

        public ActorParameters()
        {
        }

        public ActorParameters(string name)
        {
            Type = name;
        }
    }
}