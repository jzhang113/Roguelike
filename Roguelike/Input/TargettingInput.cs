using RLNET;
using System;

namespace Roguelike.Input
{
    public enum TargettingInput
    {
        None,
        JumpE,
        JumpS,
        JumpN,
        JumpW,
        JumpNW,
        JumpNE,
        JumpSW,
        JumpSE,
        MoveE,
        MoveS,
        MoveN,
        MoveW,
        MoveNW,
        MoveNE,
        MoveSW,
        MoveSE,
        NextActor,
        Fire
    }

    static partial class InputMapping
    {
        public static TargettingInput GetTargettingInput(RLKeyPress keyPress)
        {
            if (keyPress == null)
                return TargettingInput.None;

            if (keyPress.Shift)
            {
                if (KeyMap.TargettingMap.Shift.TryGetValue(keyPress.Key, out string action)
                    && Enum.TryParse(action, out TargettingInput input))
                    return input;
            }
            else
            {
                if (KeyMap.TargettingMap.None.TryGetValue(keyPress.Key, out string action)
                    && Enum.TryParse(action, out TargettingInput input))
                    return input;
            }

            return TargettingInput.None;
        }
    }
}
