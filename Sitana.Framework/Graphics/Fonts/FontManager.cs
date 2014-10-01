using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Cs;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework.Content
{
    public sealed class FontManager: Singleton<FontManager>
    {
        Dictionary<string, List<SpriteFont>> _fonts = new Dictionary<string, List<SpriteFont>>();

        public void AddFont(string fontName, string path, int size)
        {
            if (size > 256)
            {
                throw new Exception("Max font size is 256");
            }

            SpriteFont font = ContentLoader.Current.Load<SpriteFont>(path);

            List<SpriteFont> list = null;
            _fonts.TryGetValue(fontName, out list);

            if (list == null)
            {
                list = new List<SpriteFont>();
                _fonts.Add(fontName, list);
            }

            while (list.Count < size + 1)
            {
                list.Add(null);
            }

            list[size] = font;
        }

        public SpriteFont FindFont(string fontName, int size)
        {
            size = Math.Max(1,(int)(UiUnit.FontUnit * size));

            List<SpriteFont> list = null;
            _fonts.TryGetValue(fontName, out list);

            if (list.Count <= size)
            {
                return list.FindLast(f => f != null);
            }

            if (list[size] == null)
            {
                int indexNext = size+1;
                int indexPrev = size-1;

                while (indexPrev > 0 || indexNext < list.Count)
                {
                    if (indexNext < list.Count)
                    {
                        if (list[indexNext] != null)
                        {
                            return list[indexNext];
                        }
                    }

                    if (indexPrev > 0)
                    {
                        if (list[indexPrev] != null)
                        {
                            return list[indexPrev];
                        }
                    }

                    indexPrev--;
                    indexNext++;
                }
            }

            return list[size];
        }
    }
}
