using Roguelike.Interfaces;
using System;

namespace Roguelike.Core
{
    [Serializable]
    class Stair : Drawable
    {
        public Stair()
        {
            Color = Swatch.DbSky;
            Symbol = '>';
        }
    }
}
