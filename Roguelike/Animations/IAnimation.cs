using Roguelike.Core;

namespace Roguelike.Animations
{
    public interface IAnimation
    {
        LayerInfo Layer { get; }

        int Turn { get; }

        // Returns true when an animation is done updating
        bool Update();

        // Draw the animation to Layer
        void Draw();
    }
}
