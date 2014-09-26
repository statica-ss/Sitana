using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;

namespace Sitana.Framework.Ui.Views
{
    public abstract class StateDrawable<T>
    {
        public abstract void Draw(AdvancedDrawBatch drawBatch, ref Rectangle target, T state);
    }
}
