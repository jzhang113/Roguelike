using Roguelike.Actors;

namespace Roguelike.Interfaces
{
    public interface IObject
    {
        string Name { get; set; }
        IMaterial Material { get; set; }
        // IActor Carrier { get; set; }
        
        // TODO: Allow objects to be deconstructed?
        void Consume(Actor actor);
        void Apply(Actor actor);
        void Equip(Actor actor);
        void Unequip(Actor actor);
        void Attack();
        void Throw();
    }
}
