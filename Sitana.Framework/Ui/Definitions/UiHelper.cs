using System;

namespace Sitana.Framework.Ui
{
    public static class UiHelper
    {
        public static TextAlign TextAlignFromContentAlignment(HorizontalContentAlignment horz, VerticalContentAlignment vert)
        {
            TextAlign align = TextAlign.None;

            switch (horz)
            {
                case HorizontalContentAlignment.Left:
                    align |= TextAlign.Left;
                    break;

                case HorizontalContentAlignment.Center:
                    align |= TextAlign.Center;
                    break;

                case HorizontalContentAlignment.Right:
                    align |= TextAlign.Right;
                    break;

                case HorizontalContentAlignment.Auto:
                    align |= TextAlign.Center;
                    break;
            }

            switch (vert)
            {
                case VerticalContentAlignment.Top:
                    align |= TextAlign.Top;
                    break;

                case VerticalContentAlignment.Center:
                    align |= TextAlign.Middle;
                    break;

                case VerticalContentAlignment.Bottom:
                    align |= TextAlign.Bottom;
                    break;

                case VerticalContentAlignment.Auto:
                    align |= TextAlign.Middle;
                    break;
            }

            return align;
        }

        public static VerticalContentAlignment ContentAlignFromAlignment(VerticalAlignment align)
        {
            switch (align)
            {
                case VerticalAlignment.Top:
                    return VerticalContentAlignment.Top;

                case VerticalAlignment.Bottom:
                    return VerticalContentAlignment.Bottom;
            }

            return VerticalContentAlignment.Center;
        }

        public static HorizontalContentAlignment ContentAlignFromAlignment(HorizontalAlignment align)
        {
            switch (align)
            {
                case HorizontalAlignment.Left:
                    return HorizontalContentAlignment.Left;

                case HorizontalAlignment.Right:
                    return HorizontalContentAlignment.Right;
            }

            return HorizontalContentAlignment.Center;
        }
    }
}

