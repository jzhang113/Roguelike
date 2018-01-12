namespace Roguelike.Interfaces
{
    interface IEquipable
    {
        void OnEquip();
        void OnInvoke();
        void OnConsume();
        void OnRemove();
        void OnDestroy();
    }
}
