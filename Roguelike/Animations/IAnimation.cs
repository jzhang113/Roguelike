using Roguelike.Core;
using System;

namespace Roguelike.Animations
{
    public interface IAnimation
    {
        LayerInfo Layer { get; }

        // Returns true when an animation is done updating
        bool Update();

        // Draw the animation to Layer
        void Draw();

        event EventHandler Complete;
    }
}
