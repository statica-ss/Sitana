using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebatianos.Gui;
using Ebatianos.Cs;

namespace Ebatianos
{
    public static class UiTask
    {
        internal static ScreenManager _screenManager;

        public static void BeginInvoke( EmptyArgsVoidDelegate lambda )
        {
            _screenManager.UiAction(lambda);
        }
    }
}
