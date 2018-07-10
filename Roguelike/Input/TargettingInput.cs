using RLNET;
using System;

namespace Roguelike.Input
{
    public enum TargettingInput
    {
        None,
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
                if (Enum.TryParse(KeyMap.TargettingMap.Shift[keyPress.Key], out TargettingInput input))
                    return input;
            }
            else
            {
                if (Enum.TryParse(KeyMap.TargettingMap.None[keyPress.Key], out TargettingInput input))
                    return input;
            }

            return TargettingInput.None;
        }
    }
}
