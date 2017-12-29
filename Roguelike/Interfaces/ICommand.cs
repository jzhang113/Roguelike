using Roguelike.Core;

namespace Roguelike.Interfaces
{
    interface ICommand
    {
        IAction Resolve(Actor origin, Actor target);
    }
}
