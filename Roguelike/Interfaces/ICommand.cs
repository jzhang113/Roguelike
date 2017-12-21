namespace Roguelike.Interfaces
{
    interface ICommand
    {
        void Execute(Core.Actor actor);
    }
}
