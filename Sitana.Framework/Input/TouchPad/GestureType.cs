using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Input.TouchPad
{
    [Flags]
    public enum GestureType
    {
        None = 0,
        Down = 0x01,
        Up = 0x02,
        Move = 0x04,
        FreeDrag = 0x08,
        HorizontalDrag = 0x10,
        VerticalDrag = 0x20,
        Flick = 0x40,
        Tap = 0x80,
        DoubleTap = 0x100,
        HoldStart = 0x200,
        HoldCancel = 0x400,
        Hold = 0x800,
        CapturedByOther = 0x1000,
        All = 0xffff
    }
}
