namespace Roguelike.Interfaces
{
    interface ISkill
    {
        int Speed { get; }
        int Power { get; }

        void Activate();
    }
}
