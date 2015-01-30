using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Graphics;

namespace Sitana.Framework.Ui.Views
{
    public interface IBackgroundDrawable
    {
        void Draw(AdvancedDrawBatch drawBatch, Rectangle target, Color color);
    }
}
