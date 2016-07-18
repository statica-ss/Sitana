using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Globalization;
using System.Reflection;

namespace Sitana.Framework.Helpers
{
    public static class ColorParser
    {
        static Dictionary<string, Color> _colors = null;

        static ColorParser()
        {
            _colors = new Dictionary<string, Color>();

            foreach (var prop in typeof(Color).GetTypeInfo().DeclaredProperties)
            {
                if (prop.PropertyType == typeof(Color))
                {
                    string name = prop.Name.ToLowerInvariant();

                    try
                    {
                        Color color = (Color)prop.GetValue(null);
                        _colors.Add(name, color);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static Color? Parse(string name, bool premultiply = true)
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

                        return premultiply ? Color.FromNonPremultiplied(r, g, b, a) : new Color(r, g, b, a);
                    }
                }
            }

            Color parsedColor;

            if (name.Contains("*"))
            {
                string[] elements = name.Split('*');
                int alpha;

                if (!int.TryParse(elements[1].Trim(), out alpha))
                {
                    throw new Exception("Invalid format. Named color format is: Name*Alpha");
                }

                float opacity = alpha / 255.0f;

                if (_colors.TryGetValue(elements[0].Trim(), out parsedColor))
                {
                    return premultiply ? (parsedColor * opacity) : new Color(parsedColor, opacity);
                }
            }

            if (_colors.TryGetValue(name.ToLowerInvariant(), out parsedColor))
            {
                return premultiply ? parsedColor :
                    new Color(parsedColor * (255.0f / parsedColor.A), parsedColor.A / 255.0f);
            }

            string[] vals = name.Replace(" ", "").Split(',');

            if (vals.Length == 3)
            {
                if (int.TryParse(vals[0], out r) && int.TryParse(vals[1], out g) && int.TryParse(vals[2], out b))
                {
                    return new Color(r, g, b);
                }
            }

            if (vals.Length == 4)
            {
                if (int.TryParse(vals[0], out r) && int.TryParse(vals[1], out g) && int.TryParse(vals[2], out b) && int.TryParse(vals[3], out a))
                {
                    return premultiply ? Color.FromNonPremultiplied(r, g, b, a) : new Color(r, g, b, a);
                }
            }

            return null;
        }
    }
}
