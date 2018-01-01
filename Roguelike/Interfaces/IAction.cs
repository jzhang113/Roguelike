namespace Roguelike.Interfaces
{
    interface IAction
    {
        IActor Source { get; }
        int Time { get; set; }
        
        void Execute();
    }
}
