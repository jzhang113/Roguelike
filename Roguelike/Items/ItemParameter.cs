using Roguelike.Data;
using System;

namespace Roguelike.Items
{
    [Serializable]
    public class ItemParameter
    {
        public string Name { get; }
        public MaterialType Material { get; set; }

        public int AttackSpeed { get; set; } = Constants.FULL_TURN;
        public int Damage { get; set; } = Constants.DEFAULT_DAMAGE;
        public int MeleeRange { get; set; } = Constants.DEFAULT_MELEE_RANGE;
        public int ThrowRange { get; set; } = Constants.DEFAULT_THROW_RANGE;

        public ItemParameter(string name, MaterialType material)
        {
            Name = name;
            Material = material;
        }
    }
}