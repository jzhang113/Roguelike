using System.Collections.Generic;

namespace Roguelike.Data
{
    public enum MaterialType
    {
        Bamboo,
        Wood,
        Glass,
        Iron,
        Steel,
        Diamond,
        Paper,
        Flesh
    }

    struct MaterialProperty
    {
        // Density measured in kg/m^3
        public int Density { get; set; }
        public Flammability Flammability { get; set; }
    }

    static class Materials
    {
        public static IDictionary<MaterialType, MaterialProperty> MaterialList { get; set; }

        static Materials()
        {
            MaterialList = Program.LoadData<IDictionary<MaterialType, MaterialProperty>>("materials");
        }
    }
}
