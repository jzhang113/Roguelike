using Roguelike.Actors;
using Roguelike.Systems;
using System;

namespace Roguelike.Core
{
    [Serializable]
    class SaveObject
    {
        public Enums.Mode GameMode { get; set; }
        public bool ShowEquipment { get; set; }
        public bool ShowInventory { get; set; }
        public bool ShowOverlay { get; set; }

        public MapHandler Map { get; set; }
        public Random CombatRandom { get; set; }
    }
}
