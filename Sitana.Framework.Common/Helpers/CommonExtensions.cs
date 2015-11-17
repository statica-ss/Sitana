// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;


namespace Sitana.Framework
{
    public static partial class Extensions
    {
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
