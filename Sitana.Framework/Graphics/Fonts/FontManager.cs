using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Graphics;

namespace Sitana.Framework.Content
{
    public sealed class FontManager: Singleton<FontManager>
    {
        Dictionary<string, FontFace> _fonts = new Dictionary<string, FontFace>();

        public void AddSitanaFont(string name, string path, params int[] sizes)
        {
            foreach (int size in sizes)
            {
                string fontPath = String.Format("{0}{1}", path, size);
                AddSitanaFontExact(name, fontPath, size);
            }
        }

        public void AddSpriteFont(string name, string path, params int[] sizes)
        {
            foreach (int size in sizes)
            {
                string fontPath = String.Format("{0}{1}", path, size);
                AddSpriteFontExact(name, fontPath, size);
            }
        }

        public void AddSitanaFontExact(string fontName, string path, int size)
        {
            Font font = ContentLoader.Current.Load<Font>(path);
            Add(fontName, size, new UniversalFont(font));
        }

        public void AddSpriteFontExact(string fontName, string path, int size)
        {
            SpriteFont font = ContentLoader.Current.Load<SpriteFont>(path);
            Add(fontName, size, new UniversalFont(font));
        }

        private void Add(string fontName, int size, UniversalFont font)
        {
            FontFace fonts = null;
            _fonts.TryGetValue(fontName, out fonts);

            if (fonts == null)
            {
                fonts = new FontFace(fontName);
                _fonts.Add(fontName, fonts);
            }

            fonts.Add(size, font);
        }

        public FontFace FindFont(string name)
        {
            FontFace fonts;
            _fonts.TryGetValue(name, out fonts);

            return fonts;
        }

        public FontFace Font
        {
            get
            {
                return _fonts.First().Value;
            }
        }
    }
}
