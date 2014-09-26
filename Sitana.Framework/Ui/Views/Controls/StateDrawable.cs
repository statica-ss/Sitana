using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ebatianos.Ui.Views.Parameters;
using Microsoft.Xna.Framework;
using Ebatianos.Graphics;

namespace Ebatianos.Ui.Views
{
    public abstract class StateDrawable<T>
    {
        public abstract void Draw(AdvancedDrawBatch drawBatch, ref Rectangle target, T state);
    }
}
