using Roguelike.Interfaces;
using Roguelike.Utils;
using System;

namespace Roguelike.Items
{
    [Serializable]
    public class ItemParameter
    {
        public string Name { get; }
        public IMaterial Material { get; set; }

        public int AttackSpeed { get; set; } = Constants.FULL_TURN;
        public int Damage { get; set; } = Constants.DEFAULT_DAMAGE;
        public double MeleeRange { get; set; } = Constants.DEFAULT_MELEE_RANGE;
        public double ThrowRange { get; set; } = Constants.DEFAULT_THROW_RANGE;

        public ItemParameter(string name, IMaterial material)
        {
            Name = name;
            Material = material;
        }
    }
}