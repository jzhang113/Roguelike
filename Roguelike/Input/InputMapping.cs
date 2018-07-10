namespace Roguelike.Input
{
    static partial class InputMapping
    {
        private static KeyMap KeyMap { get; }

        static InputMapping()
        {
            KeyMap = Program.LoadData<KeyMap>("keyMap");
        }
    }
}
