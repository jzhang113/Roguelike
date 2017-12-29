using Roguelike.Core;

namespace Roguelike.Interfaces
{
    interface ICommand
    {
        IAction Execute(Actor origin, Actor target);
    }
}
