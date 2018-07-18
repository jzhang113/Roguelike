using RLNET;
using System;

namespace Roguelike.Input
{
    public enum NormalInput
    {
        None,
        AttackE,
        AttackS,
        AttackN,
        AttackW,
        AttackNW,
        AttackNE,
        AttackSW,
        AttackSE,
        MoveE,
        MoveS,
        MoveN,
        MoveW,
        MoveNW,
        MoveNE,
        MoveSW,
        MoveSE,
        OpenApply,
        OpenDrop,
        OpenEquip,
        OpenInventory,
        OpenUnequip,
        OpenMenu,
        AutoExplore,
        ChangeLevel,
        Get,
        Wait
    }

    static partial class InputMapping
    {
        public static NormalInput GetNormalInput(RLKeyPress keyPress)
        {
            if (keyPress == null)
                return NormalInput.None;

            if (keyPress.Shift)
            {
                if (_keyMap.NormalMap.Shift.TryGetValue(keyPress.Key, out string action)
                    && Enum.TryParse(action, out NormalInput input))
                    return input;
            }
            else
            {
                if (_keyMap.NormalMap.None.TryGetValue(keyPress.Key, out string action)
                    && Enum.TryParse(action, out NormalInput input))
                    return input;
            }

            return NormalInput.None;
        }
    }
}
