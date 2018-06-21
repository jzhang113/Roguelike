using Roguelike.Core;
using System.Collections.Generic;

namespace Roguelike.Commands
{
    interface ITargetCommand : ICommand
    {
        IEnumerable<Terrain> Target { get; set; }
    }
}
