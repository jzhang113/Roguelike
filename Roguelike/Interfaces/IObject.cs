namespace Roguelike.Interfaces
{
    public interface IObject
    {
        string Name { get; set; }
        IMaterial Material { get; set; }
        IActor Carrier { get; set; }
        
        // TODO: Allow objects to be deconstructed?
        void Consume();
        void Apply();
        void Equip();
        void Unequip();
        void Attack();
        void Throw();
    }
}
