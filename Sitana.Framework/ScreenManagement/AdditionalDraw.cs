using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ebatianos.Cs;

namespace Ebatianos.Gui
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
