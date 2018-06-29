namespace Roguelike.Core
{
    struct Configuration
    {
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public WindowConfig Screen { get; set; }
        public WindowConfig Map { get; set; }
        public WindowConfig MapView { get; set; }
        public int MessageMaxCount { get; set; }
        public WindowConfig MessageView { get; set; }
        public WindowConfig StatView { get; set; }
        public WindowConfig InventoryView { get; set; }
        public WindowConfig ViewWindow { get; set; }

        internal struct WindowConfig
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
