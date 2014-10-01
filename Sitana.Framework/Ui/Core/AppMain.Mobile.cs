// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Core
{
    public partial class AppMain
    {
        void OnSize(int width, int height)
        {
            MainView.Bounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }
    }
}
