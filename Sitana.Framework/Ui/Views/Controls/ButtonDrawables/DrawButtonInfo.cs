using System;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Ui.Views.ButtonDrawables
{
    public struct DrawButtonInfo
    {
        public SharedString Text;
        public ButtonState ButtonState;
        public Rectangle Target;
        public Texture2D Icon;
        public float Opacity;
        public float EllapsedTime;
        public object Additional;
    }
}

