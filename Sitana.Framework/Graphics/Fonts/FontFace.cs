using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Graphics
{
    public class FontFace
    {
        public readonly Dictionary<int, SpriteFont> Fonts = new Dictionary<int, SpriteFont>();
        public readonly string Name;

        public FontFace(string name)
        {
            Name = name;
        }

        internal void Add(int size, SpriteFont font)
        {
            Fonts.Add(size, font);
        }

        public SpriteFont Find(int size)
        {
            size = Math.Max(1, (int)(UiUnit.FontUnit * size));

            Dictionary<int, SpriteFont> fonts = Fonts;

            SpriteFont font = null;
            fonts.TryGetValue(size, out font);

            if (font == null)
            {
                int indexNext = size + 1;
                int indexPrev = size - 1;

                while (fonts.Count > 0)
                {
                    if (fonts.TryGetValue(indexNext, out font))
                    {
                        fonts.Add(size, font);
                        return font;
                    }

                    if (indexPrev > 0)
                    {
                        if (fonts.TryGetValue(indexPrev, out font))
                        {
                            fonts.Add(size, font);
                            return font;
                        }
                    }

                    indexPrev--;
                    indexNext++;
                }
            }

            return font;
        }
    }
}
