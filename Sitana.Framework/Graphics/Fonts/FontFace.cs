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
        public readonly Dictionary<int, Tuple<SpriteFont,int>> Fonts = new Dictionary<int, Tuple<SpriteFont,int>>();
        public readonly string Name;

        public FontFace(string name)
        {
            Name = name;
        }

        internal void Add(int size, SpriteFont font)
        {
            Fonts.Add(size, new Tuple<SpriteFont,int>(font, size));
        }

        public SpriteFont Find(int size)
        {
            size = Math.Max(1, (int)(UiUnit.FontUnit * size));

            Tuple<SpriteFont,int> font = null;
            Fonts.TryGetValue(size, out font);

            if (font == null)
            {
                int indexNext = size + 1;
                int indexPrev = size - 1;

                while (Fonts.Count > 0)
                {
                    if (Fonts.TryGetValue(indexNext, out font))
                    {
                        Fonts.Add(size, font);
                        return font.Item1;
                    }

                    if (indexPrev > 0)
                    {
                        if (Fonts.TryGetValue(indexPrev, out font))
                        {
                            Fonts.Add(size, font);
                            return font.Item1;
                        }
                    }

                    indexPrev--;
                    indexNext++;
                }
            }

            return font.Item1;
        }
    }
}
