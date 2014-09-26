using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ebatianos.Content
{
    public class SpriteFontPresenter: IFontPresenter
    {
        public SpriteFont Font { get; private set; }

        private Vector2 _size;
        private StringBuilder _text = new StringBuilder();

        #region IFontPresenter

        List<IFontPresenter> IFontPresenter.PrepareMultilineText(StringBuilder text, Int32 width, Boolean justify, Int32 indent)
        {
            return null;
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold)
        {
            DrawTextInt(spriteBatch, position, color, origin, scale, bold);
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold)
        {
            PrepareRenderInt(text, 0, text.Length);
            DrawTextInt(spriteBatch, position, color, origin, scale, bold);
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Single scale, Align align, Boolean bold)
        {
            PrepareRenderInt(text, 0, text.Length);

            Vector2 origin = CalculateOriginFromAlign(align);
            DrawTextInt(spriteBatch, position, color, origin, scale, bold);
        }

        void IFontPresenter.DrawText(SpriteBatch spriteBatch, StringBuilder text, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold)
        {
            PrepareRenderInt(text, 0, text.Length);

            Vector2 origin = CalculateOriginFromAlign(align);
            DrawTextInt(spriteBatch, position, color, origin, scale, bold);
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Single scale, Align align, Boolean bold)
        {
            Vector2 origin = CalculateOriginFromAlign(align);
            DrawTextInt(spriteBatch, position, color, origin, scale, bold);

            return _size;            
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Single scale, Align align, Boolean bold)
        {
            Vector2 origin = CalculateOriginFromAlign(align);
            DrawTextInt(spriteBatch, position, colors[0], origin, scale, bold);

            return _size;
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale, Align align, Boolean bold)
        {
            Vector2 origin = CalculateOriginFromAlign(align);
            DrawTextInt(spriteBatch, position, color, origin, scale, bold);

            return _size;
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, Vector2 position, Color[] colors, Vector2 scale, Align align, Boolean bold)
        {
            Vector2 origin = CalculateOriginFromAlign(align);
            DrawTextInt(spriteBatch, position, colors[0], origin, scale, bold);

            return _size;
        }

        Vector2 IFontPresenter.DrawText(SpriteBatch spriteBatch, String text, Vector2 position, Color color, Single scale, Align align)
        {
            PrepareRenderInt(text, 0, text.Length);

            Vector2 origin = CalculateOriginFromAlign(align);
            DrawTextInt(spriteBatch, position, color, origin, scale, false);

            return _size;
        }

        Single IFontPresenter.LineHeight
        {
            get
            {
                return Font.LineSpacing;
            }
        }

        Single IFontPresenter.Height
        {
            get
            {
                return Font.LineSpacing;
            }
        }

        Vector2 IFontPresenter.TextSize(String text)
        {
            return Font.MeasureString(_text);
        }

        Vector2 IFontPresenter.Size
        {
            get
            {
                return _size;
            }
        }

        IFontPresenter IFontPresenter.Clone()
        {
            return new SpriteFontPresenter(Font);
        }

        void IFontPresenter.PrepareRender(String text)
        {
            PrepareRenderInt(text, 0, text.Length);
        }

        void IFontPresenter.PrepareRender(String text, Boolean trimStartEnd)
        {
            PrepareRenderInt(text, 0, text.Length);
        }

        void IFontPresenter.PrepareRender(StringBuilder text, Boolean trimStartEnd)
        {
            PrepareRenderInt(text, 0, text.Length);
        }

        void IFontPresenter.PrepareRender(StringBuilder text, Int32 start, Int32 length, Int32 lineWidth)
        {
            PrepareRenderInt(text, start, length);
        }

        void IFontPresenter.PrepareRender(StringBuilder text, Int32 lineWidth)
        {
            PrepareRenderInt(text, 0, text.Length);
        }

        void IFontPresenter.PrepareRender(StringBuilder text)
        {
            PrepareRenderInt(text, 0, text.Length);
        }

        #endregion

        public SpriteFontPresenter(SpriteFont font)
        {
            Font = font;
        }

        private void PrepareRenderInt(String text, Int32 start, Int32 length)
        {
            _text.Clear();

            if (start == 0 && length == text.Length)
            {
                _text.Append(text);
            }
            else
            {
                Int32 end = start + length;

                for (Int32 idx = start; idx < end; ++idx)
                {
                    _text.Append(text[idx]);
                }
            }

            _size = Font.MeasureString(_text);
        }

        private void PrepareRenderInt(StringBuilder text, Int32 start, Int32 length)
        {
            _text.Clear();
            Int32 end = start + length;

            for (Int32 idx = start; idx < end; ++idx)
            {
                _text.Append(text[idx]);
            }

            _size = Font.MeasureString(_text);
        }

        private void DrawTextInt(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 origin, Vector2 scale, Boolean bold)
        {
            spriteBatch.DrawString(Font, _text, position, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        private void DrawTextInt(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 origin, Single scale, Boolean bold)
        {
            spriteBatch.DrawString(Font, _text, position, color, 0, origin, scale, SpriteEffects.None, 0);
        }

        private Vector2 CalculateOriginFromAlign(Align align)
        {
            Boolean center = (align & Align.Center) != Align.Left;
            Boolean right = (align & Align.Right) != Align.Left;
            Boolean vcenter = (align & Align.Middle) != Align.Left;
            Boolean bottom = (align & Align.Bottom) != Align.Left;

            Vector2 origin = Vector2.Zero;

            if (center)
            {
                origin.X = _size.X / 2;
            }
            else if (right)
            {
                origin.X = _size.X;
            }

            if (vcenter)
            {
                origin.Y = _size.Y / 2;
            }
            else if (bottom)
            {
                origin.Y = _size.Y;
            }

            return origin;
        }
    }
}
