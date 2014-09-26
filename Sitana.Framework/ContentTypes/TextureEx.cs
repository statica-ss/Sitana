using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ebatianos.Content
{
    public class TextureEx : ContentLoader.AdditionalType
    {
        

        public Int32 Width { get; private set; }
        public Int32 Height { get; private set; }

        private Single _scale;
        private Texture2D _texture;

        /// <summary>
        /// Registers additional type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(TextureEx), Load, true);
        }

        // <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            return new TextureEx(name);
        }

        public TextureEx(String name)
        {
            if (ContentLoader.Current.TextureRescaleFactor > 0)
            {
                String path = String.Format("{0}+{1}", name, ContentLoader.Current.TextureRescaleFactor);

                try
                {
                    _texture = ContentLoader.Current.Load<Texture2D>(path);
                    _scale = (Single)ContentLoader.Current.TextureRescaleFactor / 100;

                    Width = (_texture.Width * 100) / ContentLoader.Current.TextureRescaleFactor;
                    Height = (_texture.Height * 100) / ContentLoader.Current.TextureRescaleFactor;
                }
                catch { }
            }

            if (_texture == null)
            {
                _texture = ContentLoader.Current.Load<Texture2D>(name);
                _scale = 1;

                Width = _texture.Width;
                Height = _texture.Height;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle? sourceRectangle, Vector2 position, Color color, Single rotation, Vector2 origin, Single scale, SpriteEffects spriteEffects)
        {
            Draw(spriteBatch, position, sourceRectangle, color, rotation, origin, new Vector2(scale), spriteEffects);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? sourceRectangle, Color color, Single rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects)
        {
            if (_scale != 1)
            {
                origin = origin * _scale;
                scale = scale / _scale;

                if (sourceRectangle.HasValue)
                {
                    Point topLeft = GraphicsHelper.PointFromVector2(GraphicsHelper.Vector2FromPoint(sourceRectangle.Value.Location) * _scale);
                    Int32 width = (Int32)(sourceRectangle.Value.Width * _scale);
                    Int32 height = (Int32)(sourceRectangle.Value.Height * _scale);
                    
                    sourceRectangle = new Rectangle(topLeft.X, topLeft.Y, width, height);
                }
            }

            spriteBatch.Draw(_texture, position, sourceRectangle, color, rotation, origin, scale, spriteEffects, 0);
        }
    }
}
