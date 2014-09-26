// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views.Parameters
{
    public struct UiViewDrawParameters
    {
        public AdvancedDrawBatch    DrawBatch { get; set; }
        public Point                Transition { get; set; }
    }
}
