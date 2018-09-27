using Roguelike.Core;
using System;

namespace Roguelike.Animations
{
    public interface IAnimation
    {
        bool Done { get; }

        void Update();
        void Draw(LayerInfo layer);

        event EventHandler Complete;
    }
}
