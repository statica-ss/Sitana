// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sitana.Framework
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

		public static string Merge(this List<string> lines, string separator)
        {
            if (lines.Count == 0)
            {
				return string.Empty;
            }

			string output = lines[0];

            for (int idx = 1; idx < lines.Count; ++idx)
            {
                output += separator;
                output += lines[idx];
            }

            return output;
        }

        public static string Merge(this string[] lines, string separator)
        {
            return lines.Merge(separator, 0, lines.Length);
        }

        public static string Merge(this string[] lines, string separator, int start, int count)
        {
            if (lines.Length == 0)
            {
                return string.Empty;
            }

			string output = lines[start];

            for (int idx = start + 1; idx < start + count; ++idx)
            {
                output += separator;
                output += lines[idx];
            }

            return output;
        }

        public static string SafeString(this String text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            return text;
        }

        public static bool IsNullOrEmpty(this String text)
        {
            return string.IsNullOrEmpty(text);
        }

		public static bool IsNullOrWhiteSpace(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }

        public static Vector2 ToVector2(this MouseState ms)
        {
            return new Vector2(ms.X, ms.Y);
        }

        public static bool Contains(this Rectangle rect, Vector2 vec)
        {
            return rect.Contains(vec.ToPoint());
        }

        public static string[] SplitAndKeep(this string text, params char[] seperators)
        {
            int startIndex = 0;

            List<string> result = new List<string>();
            char? addChar = null;

            while (startIndex < text.Length)
            {
                int minIndex = text.Length;

                foreach (var ch in seperators)
                {
                    int index = text.IndexOf(ch, startIndex);

                    if (index >= 0)
                    {
                        minIndex = Math.Min(index, minIndex);
                    }
                }

                if (addChar.HasValue)
                {
                    result.Add(addChar.Value + text.Substring(startIndex, minIndex - startIndex));
                }
                else
                {
                    result.Add(text.Substring(startIndex, minIndex - startIndex));
                }

                if (minIndex < text.Length)
                {
                    addChar = text[minIndex];
                }

                startIndex = minIndex+1;
            }

            return result.ToArray();
        }
    }
}
