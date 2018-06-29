using Roguelike.State;
using Roguelike.World;
using System;

namespace Roguelike.Core
{
    [Serializable]
    class SaveObject
    {
        public IState GameState { get; set; }
        public bool ShowEquipment { get; set; }
        public bool ShowInventory { get; set; }
        public bool ShowOverlay { get; set; }

        public WorldHandler World { get; set; }
    }
}
