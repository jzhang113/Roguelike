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

    static class MaterialExtensions
    {
        private static readonly IDictionary<MaterialType, MaterialProperty> _materialList;

        static MaterialExtensions()
        {
            _materialList = Program.LoadData<IDictionary<MaterialType, MaterialProperty>>("materials");
        }

        public static MaterialProperty ToProperty(this MaterialType type) => _materialList[type];
    }
}
