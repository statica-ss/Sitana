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
        public readonly Dictionary<int, Tuple<Font, int>> Fonts = new Dictionary<int, Tuple<Font, int>>();
        public readonly string Name;

        int _maxSize = 0;
        int _minSize = int.MaxValue;

        public FontFace(string name)
        {
            Name = name;
        }

        internal void Add(int size, Font font)
        {
            Fonts.Add(size, new Tuple<Font,int>(font, size));
            _minSize = Math.Min(size, _minSize);
            _maxSize = Math.Max(size, _maxSize);
        }

        public Font Find(int size, out float scale)
        {
            size = Math.Max(1, (int)(UiUnit.FontUnit * size));

            Tuple<Font, int> font = null;
            Fonts.TryGetValue(size, out font);

            scale = 1;

            if (UiUnit.EnableFontScaling)
            {
                if (font == null)
                {
                    int indexNext = size + 1;

                    while (indexNext <= _maxSize)
                    {
                        if (Fonts.TryGetValue(indexNext, out font))
                        {
                            Fonts.Add(size, font);
                            break;
                        }

                        indexNext++;
                    }
                }

                if (font == null)
                {
                    int indexPrev = size - 1;

                    while (indexPrev >= _minSize)
                    {
                        if (Fonts.TryGetValue(indexPrev, out font))
                        {
                            Fonts.Add(size, font);
                            break;
                        }

                        indexPrev--;
                    }
                }

                scale = (float)size / (float)font.Item2;
            }
            else
            {
                if (font == null)
                {
                    int indexNext = size + 1;
                    int indexPrev = size - 1;

                    while (indexNext <= _maxSize || indexPrev >= _minSize)
                    {
                        if (Fonts.TryGetValue(indexNext, out font))
                        {
                            Fonts.Add(size, font);
                            break;
                        }

                        indexNext++;

                        if (Fonts.TryGetValue(indexPrev, out font))
                        {
                            Fonts.Add(size, font);
                            break;
                        }

                        indexPrev--;
                    }
                }
            }

            return font.Item1;
        }
    }
}
