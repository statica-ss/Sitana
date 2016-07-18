using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Core;
using System.Text;

namespace Sitana.Framework.Graphics
{
    public class UiFont
    {
        private readonly FontFace _fontFace;
        private readonly double _fontSize;

        private UniversalFont _universalFont;
        private float _scale;
        private float _spacing = 0;
        private float _lineHeight = 1;

        private double _lastUnit = 0;

        public UniversalFont Font
        {
            get
            {
                Get();
                return _universalFont;
            }
        }

        public float Scale
        {
            get
            {
                Get();
                return _scale;
            }
        }

        public float Spacing
        {
            get
            {
                return _spacing;
            }
        }

        public float LineHeight
        {
            get
            {
                return _lineHeight;
            }
        }

        public UiFont(string font, double size, int spacing): this(font, size)
        {
            _spacing = spacing / 1000.0f;
        }

        public UiFont(string font, double size, int spacing, int lineHeight): this(font, size, spacing)
        {
            _lineHeight = lineHeight / 100.0f;
        }

        public UiFont(string font, double size)
        {
            _fontFace = FontManager.Instance.FindFont(font);
            _fontSize = size;
        }

        public Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text, Spacing, LineHeight) * Scale;
        }

        public Vector2 MeasureString(StringBuilder text)
        {
            return Font.MeasureString(text, Spacing, LineHeight) * Scale;
        }

        public Vector2 MeasureString(SharedString text)
        {
            lock (text)
            {
                return Font.MeasureString(text.StringBuilder, Spacing, LineHeight) * Scale;
            }
        }

        void Get()
        {
            if (UiUnit.FontUnit != _lastUnit)
            {
                int intSize = (int)_fontSize;
                double mul = _fontSize / intSize;

                _universalFont = _fontFace.Find(intSize, out _scale);
                _scale *= (float)mul;
                _lastUnit = UiUnit.FontUnit;
            }
        }
    }
}
