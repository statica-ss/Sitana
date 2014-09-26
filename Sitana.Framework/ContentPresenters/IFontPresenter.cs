using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Content
{
    public class PrepareRenderException : Exception
    {
        public PrepareRenderException(String text)
            : base(text)
        { }
    }

    public interface IFontPresenter
    {
        IFontPresenter Clone();
        void PrepareRender(String text);
        void PrepareRender(String text, Boolean trimStartEnd);
        void PrepareRender(StringBuilder text, Boolean trimStartEnd);
        void PrepareRender(StringBuilder text, Int32 start, Int32 length, Int32 lineWidth);
        void PrepareRender(StringBuilder text, Int32 lineWidth);
        void PrepareRender(StringBuilder text);

        Single LineHeight { get; }
        Single Height { get; }
        Vector2 Size { get; }

        Vector2 TextSize(String text);

        void DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold = false);
        void DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold = false);
        void DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Single scale, Align align, Boolean bold = false);
        void DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold = false);
        Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Single scale, Align align, Boolean bold = false);        
        Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Single scale, Align align, Boolean bold = false);
        Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold = false);
        Vector2 DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Vector2 scale, Align align, Boolean bold = false);
        Vector2 DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Single scale, Align align);

        List<IFontPresenter> PrepareMultilineText(StringBuilder text, Int32 width, Boolean justify = true, Int32 indent = 0);
    }
}
