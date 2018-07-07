using Roguelike.State;
using Roguelike.World;
using System;

namespace Roguelike.Core
{
    [Serializable]
    class SaveObject
    {
        public IState GameState { get; set; }

        public WorldHandler World { get; set; }
    }
}
