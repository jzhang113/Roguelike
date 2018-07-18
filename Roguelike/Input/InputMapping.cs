namespace Roguelike.Input
{
    static partial class InputMapping
    {
        private static readonly KeyMap _keyMap;

        static InputMapping()
        {
            _keyMap = Program.LoadData<KeyMap>("keyMap");
        }
    }
}
