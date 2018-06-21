namespace Roguelike.Commands
{
    interface IInputCommand : ICommand
    {
        string Input { get; set; }
    }
}
