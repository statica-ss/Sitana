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
        public readonly Dictionary<int, Tuple<UniversalFont, int>> Fonts = new Dictionary<int, Tuple<UniversalFont, int>>();
        public readonly string Name;

        int _maxSize = 0;
        int _minSize = int.MaxValue;

        public FontFace(string name)
        {
            Name = name;
        }

        internal void Add(int size, UniversalFont font)
        {
            Fonts.Add(size, new Tuple<UniversalFont, int>(font, size));
            _minSize = Math.Min(size, _minSize);
            _maxSize = Math.Max(size, _maxSize);
        }

        public UniversalFont Find(int size, out float scale)
        {
            size = Math.Max(1, (int)(UiUnit.FontUnit * size));

            Tuple<UniversalFont, int> font = null;
            Fonts.TryGetValue(size, out font);

            scale = 1;

            if (UiUnit.FontScaling != UiUnit.ScalingMode.None)
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

                if (UiUnit.FontScaling == UiUnit.ScalingMode.Integer)
                {
                    scale = (int)Math.Round(scale);
                }
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
