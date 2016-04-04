using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.Core
{
    public static class UiUnit
    {
        public enum ScalingMode
        {
            None,
            Floating,
            Integer
        }

        public static double Unit = 1;
        public static double FontUnit = 1;
        public static double GameUnit = 1;
        public static double PixelsPerMm = 1;

        public static ScalingMode FontScaling = ScalingMode.None;
    }
}
