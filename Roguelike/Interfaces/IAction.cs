namespace Roguelike.Interfaces
{
    interface IAction
    {
        int Time { get; set; }
        
        void Execute();
    }
}
