using RLNET;
using System.Collections.Generic;

namespace Roguelike.Input
{
    struct KeyMap
    {
        public StateMap NormalMap { get; set; }
        public StateMap TargettingMap { get; set; }

        internal struct StateMap
        {
            public IDictionary<RLKey, string> Shift { get; set; }
            public IDictionary<RLKey, string> Ctrl { get; set; }
            public IDictionary<RLKey, string> Alt { get; set; }
            public IDictionary<RLKey, string> None { get; set; }
        }
    }
}
