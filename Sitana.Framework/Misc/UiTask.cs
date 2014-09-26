using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Gui;
using Sitana.Framework.Cs;

namespace Sitana.Framework
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
