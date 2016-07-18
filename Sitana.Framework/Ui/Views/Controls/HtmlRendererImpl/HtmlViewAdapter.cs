using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Helpers;
using Sitana.Framework.Ui.Views;
using System;
using System.IO;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public class HtmlViewAdapter : RAdapter
    {
        public static readonly HtmlViewAdapter Instance = new HtmlViewAdapter();

        private HtmlViewAdapter()
        {

        }

        protected override RImage ConvertImageInt(object image)
        {
            return image != null ? new HtmlViewImage((Texture2D)image) : null;
        }

        protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style)
        {
            return CreateFontInt(family.Name, size, style);
        }

        protected override RFont CreateFontInt(string family, double size, RFontStyle style)
        {
            return new HtmlViewFont(family, size, style);
        }

        protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
        {
            return new HtmlViewBrush(color1);
        }

        protected override RPen CreatePen(RColor color)
        {
            return new HtmlViewPen(color);
        }

        protected override RBrush CreateSolidBrush(RColor color)
        {
            return new HtmlViewBrush(color);
        }

        protected override RColor GetColorInt(string colorName)
        {
            var parsedValue = ColorParser.Parse(colorName);

            if(colorName=="solid")
            {
                return RColor.Black;
            }

            if(parsedValue.HasValue)
            {
                return parsedValue.Value.ToHtmlRendererColor();
            }

            return RColor.White;
        }

        protected override RImage ImageFromStreamInt(Stream memoryStream)
        {
            return new HtmlViewImage(memoryStream);
        }
    }
}
