using System;

namespace Roguelike.Animations
{
    public interface IAnimation
    {
        bool Done { get; }

        void Update();
        void Draw();

        event EventHandler Complete;
    }
}
