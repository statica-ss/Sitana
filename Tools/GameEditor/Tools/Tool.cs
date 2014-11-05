using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameEditor.Tools
{
    abstract class Tool
    {
        public static Tool Current = null;

        protected Tool()
        {
            Current = this;
        }

        public virtual void Draw(AdvancedDrawBatch batch, Point position, float scale)
        {
        }
    }
}
