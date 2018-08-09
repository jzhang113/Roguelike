using MessagePack;
using RLNET;
using System;

namespace Roguelike.Animations
{
    [Union(0, typeof(ExplosionAnimation))]
    [Union(1, typeof(HookAnimation))]
    [Union(2, typeof(SpinAnimation))]
    [Union(3, typeof(TrailAnimation))]
    public interface IAnimation
    {
        bool Done { get; }

        void Update();
        void Draw(RLConsole console);

        event EventHandler Complete;
    }
}
