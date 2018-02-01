namespace Roguelike.Interfaces
{
    public interface ISkill
    {
        int Speed { get; }
        int Power { get; }

        void Activate();
    }
}
