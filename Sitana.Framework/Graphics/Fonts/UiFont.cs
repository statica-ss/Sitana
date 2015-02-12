using Sitana.Framework.Content;
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

        public UiFont(string font, int size)
        {
            _fontFace = FontManager.Instance.FindFont(font);
            _fontSize = size;
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
