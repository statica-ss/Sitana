// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;

namespace Sitana.Framework.Ui.Views
{
    public abstract class StateDrawable<T>
    {
        public abstract void Draw(AdvancedDrawBatch drawBatch, ref Rectangle target, T state);
    }
}
