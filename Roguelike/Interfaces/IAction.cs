namespace Roguelike.Interfaces
{
    interface IAction
    {
        IActor Source { get; }
        int EnergyCost { get; set; }
        
        void Execute();
    }
}
