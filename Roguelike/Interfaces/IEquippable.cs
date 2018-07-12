namespace Roguelike.Interfaces
{
    // Represents items that can be equipped.
    interface IEquippable
    {
        // Unequip existing items and equip this item.
        void Equip(IEquipped equipped);

        // Unequip the item. Fails if the item is already unequipped.
        void Unequip(IEquipped equipped);
    }
}
