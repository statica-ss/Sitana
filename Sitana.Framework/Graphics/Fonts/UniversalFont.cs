using Microsoft.Xna.Framework.Graphics;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public class UniversalFont
    {
        public readonly SpriteFont SpriteFont;
        public readonly Font SitanaFont;

        public UniversalFont(SpriteFont font)
        {
            SpriteFont = font;
            SitanaFont = null;
        }

        public UniversalFont(Font font)
        {
            SitanaFont = font;
            SpriteFont = null;
        }

        public Vector2 MeasureString(StringBuilder text)
        {
            return SitanaFont != null ? SitanaFont.MeasureString(text) : SpriteFont.MeasureString(text);
        }

        public Vector2 MeasureString(string text)
        {
            return SitanaFont != null ? SitanaFont.MeasureString(text) : SpriteFont.MeasureString(text);
        }
    }
}
