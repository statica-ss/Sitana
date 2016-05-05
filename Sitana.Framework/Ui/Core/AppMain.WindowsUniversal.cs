// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using Windows.Graphics.Display;

namespace Sitana.Framework.Ui.Core
{
    public partial class AppMain
    {
		void PlatformInit()
        {
            DefinedSymbols.Define("uwp");
            DefinedSymbols.Define("win");

            double dpi = Math.Max(DisplayInformation.GetForCurrentView().RawDpiX, DisplayInformation.GetForCurrentView().RawDpiY);

            double px = dpi / 25.4;
            UiUnit.PixelsPerMm = px;
        }
    }
}
