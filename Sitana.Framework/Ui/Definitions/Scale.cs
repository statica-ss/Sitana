using Sitana.Framework.Ui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitana.Framework.Ui
{
    public struct Scale
    {
        public static readonly Scale One = new Scale(1, 0);

        double _scale;
        double _phisicalScale;

        public float Value(bool scaleByUnit)
        {
            return (float)(UiUnit.PhisicalScale * _phisicalScale + _scale * (scaleByUnit ? UiUnit.Unit : 1));
        }

        public Scale(double scale, double phisicalScale = 0)
        {
            _scale = scale;
            _phisicalScale = phisicalScale;
        }
    }
}
