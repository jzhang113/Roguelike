using Roguelike.Actors;

namespace Roguelike.Interfaces
{
    // Represents items that can be equipped.
    interface IEquipable
    {
        // Unequip existing items and equip this item.
        void Equip();

        // Unequip the item. Fails if the item is already unequipped.
        void Unequip();
    }
}
