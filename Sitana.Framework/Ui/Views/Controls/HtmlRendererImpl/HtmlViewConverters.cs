using Microsoft.Xna.Framework;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace Sitana.Framework.Ui.Views.HtmlRendererImpl
{
    public static class HtmlViewConverters
    {
        public static Color ToXnaColor(this RColor color)
        {
            return Color.FromNonPremultiplied(color.R, color.G, color.B, color.A);
        }

        public static RColor ToHtmlRendererColor(this Color color)
        {
            color = color * (255.0f / color.A);
            return RColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static RPoint ToHtmlRendererPoint(this Point point)
        {
            return new RPoint(point.X, point.Y);
        }

        public static RRect ToHtmlRendererRect(this Rectangle rect)
        {
            return new RRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rectangle ToXnaRectangle(this RRect rect)
        {
            return new Rectangle(
                (int)rect.X,
                (int)rect.Y,
                (int)rect.Width,
                (int)rect.Height
                );
        }
    }
}
