using Roguelike.Interfaces;
using Roguelike.World;
using System;

namespace Roguelike.Core
{
    [Serializable]
    public class Exit
    {
        public LevelId Destination { get; }
        public Drawable DrawingComponent { get; }

        public int X { get => DrawingComponent.X; set => DrawingComponent.X = value; }
        public int Y { get => DrawingComponent.Y; set => DrawingComponent.Y = value; }

        public Exit(LevelId destination)
        {
            Destination = destination;
            DrawingComponent = new Drawable(Colors.Exit, '*', true);
        }
    }
}
