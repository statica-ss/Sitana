using System;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public struct DrawButtonInfo
    {
        public SharedString Text;
        public ButtonState ButtonState;
        public Rectangle Target;
        public float Opacity;
        public float EllapsedTime;
    }
}

