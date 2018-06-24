using Roguelike.Interfaces;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Stair
    {
        public string Destination { get; }
        public Drawable DrawingComponent { get; }

        public int X { get => DrawingComponent.X; set => DrawingComponent.X = value; }
        public int Y { get => DrawingComponent.Y; set => DrawingComponent.Y = value; }

        public Stair(string destination)
        {
            DrawingComponent = new Drawable
            {
                Color = Swatch.DbSky,
                Symbol = '>'
            };

            System.Diagnostics.Debug.Assert(Game.World.IsValidLevel(destination));
            Destination = destination;
        }
    }
}
