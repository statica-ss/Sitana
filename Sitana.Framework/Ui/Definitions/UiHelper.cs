using System;

namespace Sitana.Framework.Ui
{
    public static class UiHelper
    {
        public static TextAlign TextAlignFromAlignment(HorizontalAlignment horz, VerticalAlignment vert)
        {
            TextAlign align = TextAlign.None;

            switch(horz)
            {
            case HorizontalAlignment.Left:
                align |= TextAlign.Left;
                break;

            case HorizontalAlignment.Center:
                align |= TextAlign.Center;
                break;

            case HorizontalAlignment.Right:
                align |= TextAlign.Right;
                break;

            case HorizontalAlignment.Stretch:
                align |= TextAlign.Center;
                break;
            }

            switch(vert)
            {
            case VerticalAlignment.Top:
                align |= TextAlign.Top;
                break;

            case VerticalAlignment.Center:
                align |= TextAlign.Middle;
                break;

            case VerticalAlignment.Bottom:
                align |= TextAlign.Bottom;
                break;

            case VerticalAlignment.Stretch:
                align |= TextAlign.Middle;
                break;
            }

            return align;
        }
    }
}

