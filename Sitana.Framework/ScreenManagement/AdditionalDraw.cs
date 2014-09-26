using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Cs;

namespace Sitana.Framework.Gui
{
    public struct AdditionalDraw
    {
        public EmptyArgsVoidDelegate DrawFunction {get; private set;}
        public Rectangle Clip {get; private set;}

        public AdditionalDraw(EmptyArgsVoidDelegate drawFunction, Rectangle clip ):this()
        {
            DrawFunction = drawFunction;
            Clip = clip;
        }
    }
}
