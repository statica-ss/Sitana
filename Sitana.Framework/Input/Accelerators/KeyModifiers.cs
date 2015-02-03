using System;

namespace Sitana.Framework.Input
{
    [Flags]
    public enum KeyModifiers
    {
        None = 0,
        Shift = 1,
        Ctrl = 2,
        Alt = 4,
        CtrlAlt = Ctrl | Alt,
        CtrlShift = Ctrl | Shift,
        ShiftAlt = Shift | Alt,
        ShiftCtrlAlt = Shift | Alt | Ctrl
    }
}

