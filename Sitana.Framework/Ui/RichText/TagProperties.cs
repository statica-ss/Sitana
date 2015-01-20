using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Ui.RichText
{
    public struct TagProperties
    {
        public string Url;
        public int IndentLevel;
        public int ListIndex;

        public SizeType FontSize;
        public FontType FontType;

        public bool IsTight;

        public static TagProperties Default = new TagProperties() { Url = null, IndentLevel = 0, ListIndex = 0, FontSize = SizeType.p, FontType = FontType.p, IsTight=false };
    }
}
