using Microsoft.Xna.Framework;
using Sitana.Framework.Content;
using Sitana.Framework.Cs;
using Sitana.Framework.Ui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Graphics
{
    public class UiFont
    {
        private readonly FontFace _fontFace;
        private readonly int _fontSize;

        private UniversalFont _universalFont;
        private float _scale;
        private float _spacing = 0;

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

        public UiFont(string font, int size, int spacing): this(font, size)
        {
            _spacing = (float)spacing / 1000.0f;
        }

        public UiFont(string font, int size)
        {
            _fontFace = FontManager.Instance.FindFont(font);
            _fontSize = size;
        }

        public Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text, Spacing) * Scale;
        }

        public Vector2 MeasureString(StringBuilder text)
        {
            return Font.MeasureString(text, Spacing) * Scale;
        }

        public Vector2 MeasureString(SharedString text)
        {
            lock (text)
            {
                return Font.MeasureString(text.StringBuilder, Spacing) * Scale;
            }
        }

        void Get()
        {
            if (UiUnit.FontUnit != _lastUnit)
            {
                _universalFont = _fontFace.Find(_fontSize, out _scale);
                _lastUnit = UiUnit.FontUnit;
            }
        }
    }
}
