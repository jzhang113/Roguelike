using Roguelike.Actors;
using Roguelike.Items;
using Roguelike.Systems;
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

        public bool IsWalkable { get => !IsWall && !IsOccupied; }
        public bool IsLightable { get => !IsWall && !BlocksLight; }

        public Terrain(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}