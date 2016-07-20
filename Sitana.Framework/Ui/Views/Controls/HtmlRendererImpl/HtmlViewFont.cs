using Sitana.Framework.Content;
using Sitana.Framework.Graphics;
using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewFont: RFont
    {
        public static string DefaultFont = "Segoe UI";

        public static double DefaultLineHeight = 1;
        public static double DefaultSpacing = 0;

        public static string BoldSuffix = "Bold";
        public static string ItalicSuffix = "Italic";

        UiFont _font;
        double _size;

        public UiFont UiFont { get { return _font; } }

        public HtmlViewFont(string name, double size, RFontStyle style)
        {
            string suffix = "";

            if ((style & RFontStyle.Bold) != 0)
            {
                suffix += BoldSuffix;
            }

            if ((style & RFontStyle.Italic) != 0)
            {
                suffix += ItalicSuffix;
            }

            var fontFace = FontManager.Instance.FindFont(name);

            if(fontFace == null)
            {
                name = DefaultFont;
            }

            name = name + suffix;

            fontFace = FontManager.Instance.FindFont(name);

            if (fontFace == null)
            {
                name = DefaultFont;
            }

            _font = new UiFont(name, size,(int)(DefaultSpacing * 1000), (int)(DefaultLineHeight * 100));
            _size = size;
        }

        public override double Height
        {
            get
            {
                return _font.LineHeight * _font.Font.Height * _font.Scale;
            }
        }

        public override double LeftPadding
        {
            get
            {
                return 0;
            }
        }

        public override double Size
        {
            get
            {
                return _size;
            }
        }

        public override double UnderlineOffset
        {
            get
            {
                return _font.LineHeight * 1.1;
            }
        }

        public override double GetWhitespaceWidth(RGraphics graphics)
        {
            return _font.MeasureString(" ").X;
        }
    }
}
