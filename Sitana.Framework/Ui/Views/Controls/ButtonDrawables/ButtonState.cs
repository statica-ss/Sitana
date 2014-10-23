using System;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    [Flags]
    public enum ButtonState
    {
        None,
        Disabled = 0x2,
        Pushed = 0x1,
        Mask = 0xf,
        Checked = 0x10
    }
}

