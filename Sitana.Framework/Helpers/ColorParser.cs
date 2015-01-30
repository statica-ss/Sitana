using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace Sitana.Framework.Helpers
{
    public static class ColorParser
    {
        public static Color? Parse(string name)
        {
            int r, g, b, a;

            if (name.StartsWith("#"))
            {
                if (name.Length == 7 || name.Length == 9)
                {
                    int color;
                    if (int.TryParse(name.Replace("#", ""), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out color))
                    {
                        a = (color >> 24) & 0xff;
                        r = (color >> 16) & 0xff;
                        g = (color >> 8) & 0xff;
                        b = color & 0xff;

                        if (name.Length == 7)
                        {
                            a = 255;
                        }

                        return Color.FromNonPremultiplied(r, g, b, a);
                    }
                }
            }

            string[] vals = name.Replace(" ", "").Split(',');

            if (vals.Length == 3)
            {
                if (Int32.TryParse(vals[0], out r) && Int32.TryParse(vals[1], out g) && Int32.TryParse(vals[2], out b))
                {
                    return new Color(r, g, b);
                }
            }

            if (vals.Length == 4)
            {
                if (Int32.TryParse(vals[0], out r) && Int32.TryParse(vals[1], out g) && Int32.TryParse(vals[2], out b) && Int32.TryParse(vals[3], out a))
                {
                    return Color.FromNonPremultiplied(r, g, b, a);
                }
            }

            return null;
        }
    }
}
