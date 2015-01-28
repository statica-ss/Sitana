using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Views.Controls.ButtonDrawables
{
    [Flags]
    public enum EditState
    {
        None,
        Invalid = 0x200,
        Required = 0x400
    }
}
