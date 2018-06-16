using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Terrain
    {
        public int X { get; }
        public int Y { get; }

        public bool IsExplored { get; internal set; }
        public bool IsWall { get; internal set; }
        public bool IsVisible { get; internal set; }
        public bool IsOccupied { get; internal set; }
        public bool BlocksLight { get; internal set; }

        public bool IsWalkable => !IsWall && !IsOccupied;
        public bool IsLightable => !IsWall && !BlocksLight;

        public Terrain(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}