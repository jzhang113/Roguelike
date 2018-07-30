using RLNET;
using System;

namespace Roguelike.Animations
{
    public interface IAnimation
    {
        bool Done { get; }

        void Update();
        void Draw(RLConsole console);

        event EventHandler Complete;
    }
}
