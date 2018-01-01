using Roguelike.Core;

namespace Roguelike.Interfaces
{
    interface ICommand
    {
        IAction Resolve(IActor origin, IActor target);
    }
}
