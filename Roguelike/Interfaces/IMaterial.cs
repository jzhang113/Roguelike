﻿namespace Roguelike.Interfaces
{
    static class Materials
    {
        public static IMaterial Wood = new IMaterial
        {
            MaterialType = MaterialTypes.Wood,
            Strength = 10,
            Durability = 10
        };

        public static IMaterial Glass = new IMaterial
        {
            MaterialType = MaterialTypes.Glass,
            Strength = 20,
            Durability = 5
        };

        public static IMaterial Iron = new IMaterial
        {
            MaterialType = MaterialTypes.Iron,
            Strength = 20,
            Durability = 20
        };

        public static IMaterial Steel = new IMaterial
        {
            MaterialType = MaterialTypes.Steel,
            Strength = 25,
            Durability = 40
        };

        public static IMaterial Diamond = new IMaterial
        {
            MaterialType = MaterialTypes.Diamond,
            Strength = 100,
            Durability = 100
        };
    }

    public class IMaterial
    {
        public MaterialTypes MaterialType { get; set; }
        public int Strength { get; set; }
        public int Durability { get; set; }
    }

    public enum MaterialTypes
    {
        Wood,
        Glass,
        Iron,
        Steel,
        Diamond
    }
}