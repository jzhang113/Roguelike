using Roguelike.Core;

namespace Roguelike.Interfaces
{
    interface ICommand
    {
        void Execute(Actor origin, Actor target);
        string Message(Actor origin);
    }
}
