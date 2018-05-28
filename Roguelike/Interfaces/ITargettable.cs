using Roguelike.Core;
using System.Collections.Generic;

namespace Roguelike.Interfaces
{
    interface ITargettable
    {
        IEnumerable<Terrain> Target { get; set; }
    }
}
