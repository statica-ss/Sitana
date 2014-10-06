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

        public void AddFont(string name, string path, int[] sizes)
        {
            foreach (int size in sizes)
            {
                string fontPath = String.Format("{0}{1}", path, size);
                AddFont(name, fontPath, size);
            }
        }

        public void AddFont(string fontName, string path, int size)
        {
            Font font = ContentLoader.Current.Load<Font>(path);

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
    }
}
