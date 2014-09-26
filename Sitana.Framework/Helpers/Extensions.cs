using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Ebatianos
{
    public static class Extensions
    {
        public static Point ToPoint(this Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public static Vector2 ToVector2(this Point pt)
        {
            return new Vector2(pt.X, pt.Y);
        }

        public static Vector2 TrimToIntValues(this Vector2 vec)
        {
            return new Vector2((int)vec.X, (int)vec.Y);
        }

        public static int ToInt(this Color color)
        {
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }

        public static Color ToColor(this int color)
        {
            int a = (color >> 24) & 0xff;
            int r = (color >> 16) & 0xff;
            int g = (color >> 8) & 0xff;
            int b = (color) & 0xff;

            return Color.FromNonPremultiplied(r,g,b,a);
        }

        public static String Merge(this List<String> lines, String separator)
        {
            if (lines.Count == 0)
            {
                return String.Empty;
            }

            String output = lines[0];

            for (Int32 idx = 1; idx < lines.Count; ++idx)
            {
                output += separator;
                output += lines[idx];
            }

            return output;
        }

        public static String Merge(this String[] lines, String separator)
        {
            if (lines.Length == 0)
            {
                return String.Empty;
            }

            String output = lines[0];

            for (Int32 idx = 1; idx < lines.Length; ++idx)
            {
                output += separator;
                output += lines[idx];
            }

            return output;
        }

        public static String SafeString(this String text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            return text;
        }

        public static Boolean IsNullOrEmpty(this String text)
        {
            return String.IsNullOrEmpty(text);
        }

        public static Boolean IsNullOrWhiteSpace(this String text)
        {
            return String.IsNullOrWhiteSpace(text);
        }

        public static String UrlEncode(this byte[] data)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte d in data)
            {
                sb.AppendFormat("%{0:X02}", d);
            }

            return sb.ToString();
        }

        public static Vector2 ToVector2(this MouseState ms)
        {
            return new Vector2(ms.X, ms.Y);
        }

        public static bool Contains(this Rectangle rect, Vector2 vec)
        {
            return rect.Contains(vec.ToPoint());
        }
    }
}
