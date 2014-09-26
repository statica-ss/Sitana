using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;
using Sitana.Framework.Cs;

namespace Sitana.Framework
{
    public class ColorWrapper: SharedValue<Color>
    {
        public ColorWrapper()
        {
            Value = Color.White;
        }

        public ColorWrapper(Color color)
        {
            Value = color;
        }

        public static implicit operator ColorWrapper(Color color)
        {
            return new ColorWrapper(color);
        }
    }
}
