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

        public Vector2 MeasureString(StringBuilder text, float spacing = 0, float lineHeight = 1)
        {
            return SitanaFont != null ? SitanaFont.MeasureString(text, spacing, lineHeight) : SpriteFont.MeasureString(text);
        }

        public Vector2 MeasureString(string text, float spacing = 0, float lineHeight = 1)
        {
            return SitanaFont != null ? SitanaFont.MeasureString(text, spacing, lineHeight) : SpriteFont.MeasureString(text);
        }

        public int Height
        {
            get
            {
                return SitanaFont != null ? SitanaFont.Height : (int)SpriteFont.MeasureString("A").Y;
            }
        }
    }
}
