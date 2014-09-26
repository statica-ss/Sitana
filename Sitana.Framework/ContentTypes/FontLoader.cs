using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Sitana.Framework.Content
{
    public static class FontLoader
    {
        public static IFontPresenter Load(String path, String directory)
        {
            return Load(path);
        }

        public static IFontPresenter Load(String path)
        {
            try
            {
                SpriteFont font = ContentLoader.Current.Load<SpriteFont>(path);
                return new SpriteFontPresenter(font);
            }
            catch
            {
                String[] names = path.Split(';');
                BitmapFont font = ContentLoader.Current.Load<BitmapFont>(names[0]);

                return new BitmapFontPresenter(font, names);
            }
        }
    }
}
