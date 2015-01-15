using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Text
{
    public static class HtmlSpecialChars
    {
        static string nbsp = ((char)0xa0).ToString();
        static string copy = ((char)0xa9).ToString();
        static string reg = ((char)0xae).ToString();

        public static string Convert(string withChars)
        {
            StringBuilder sb = new StringBuilder(withChars);

            return sb
                  .Replace("&nbsp;", nbsp)
                  .Replace("&amp;", "&")
                  .Replace("&copy;", copy)
                  .Replace("&reg;", reg)
                  .Replace("&quot;", "\"")
                  .ToString();
        }
    }
}
