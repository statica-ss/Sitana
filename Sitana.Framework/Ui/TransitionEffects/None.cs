using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Ui.Views;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views.TransitionEffects
{
    public class None : TransitionEffect
    {
        public override void Get(double transition, Rectangle containerRect, Rectangle elementRect, out Matrix transform, out float opacity)
        {
            transform = Matrix.Identity;
            opacity = 1;
        }

        public override TransitionEffect Reverse()
        {
            return this;
        }
    }
    
}
