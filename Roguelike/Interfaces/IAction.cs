namespace Roguelike.Interfaces
{
    interface IAction
    {
        void Execute(IActor target);
    }
}
