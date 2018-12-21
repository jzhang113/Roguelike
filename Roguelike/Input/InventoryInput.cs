namespace Roguelike.Input
{
    internal enum InventoryInput
    {
        None,
        Open,
        MoveUp,
        MoveDown,
        OpenLetter
    }

    internal static partial class InputMapping
    {
        public static InventoryInput GetInventoryInput(int key)
        {
            if (_keyMap.InventoryMap.None.TryGetValue(key, out InventoryInput action))
                return action;
            else if (key >= 0x04 && key < 0x1D)
                return InventoryInput.OpenLetter;
            else
                return InventoryInput.None;
        }
    }
}
