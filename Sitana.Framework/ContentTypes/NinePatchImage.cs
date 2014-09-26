using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Sitana.Framework.Content
{
    public class NinePatchImage : ContentLoader.AdditionalType
    {
        private Texture2D _texture;

        private Rectangle[] _rectangles;

        public Int32 Width { get; private set; }
        public Int32 Height { get; private set; }

        private Boolean _drawOnlyInside = false;

        internal Texture2D Texture
        {
            get
            {
                return _texture;
            }
        }

        /// <summary>
        /// Registers type in ContentLoader
        /// </summary>
        public static void Register()
        {
            RegisterType(typeof(NinePatchImage), Load, true);
        }

        /// <summary>
        /// Loads content object
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            String directory = Path.GetDirectoryName(name);
            XNode node = null;

            try
            {
                node = ContentLoader.Current.Load<XFile>(name);
            }
            catch
            {
            }

            if (node != null)
            {

                if (node.Tag != "NinePatch")
                {
                    throw new InvalidDataException("Invalid node name for Nine Patch Image.");
                }

                String textureName = node.Attribute("Texture");

                Texture2D texture = ContentLoader.Current.Load<Texture2D>(textureName);

                Rectangle source = StringParser.ParseRectangle(node.Attribute("Source"));
                Rectangle scale = StringParser.ParseRectangle(node.Attribute("Scalable"));

                Rectangle[] rects = new Rectangle[9];

                Int32 width = source.Width;
                Int32 height = source.Height;

                rects[0] = new Rectangle(0, 0, scale.X, scale.Y);
                rects[1] = new Rectangle(scale.X, 0, scale.Width, scale.Y);
                rects[2] = new Rectangle(scale.Right, 0, width - scale.Right, scale.Y);

                rects[3] = new Rectangle(0, scale.Y, scale.X, scale.Height);
                rects[4] = new Rectangle(scale.X, scale.Y, scale.Width, scale.Height);
                rects[5] = new Rectangle(scale.Right, scale.Y, width - scale.Right, scale.Height);

                rects[6] = new Rectangle(0, scale.Bottom, scale.X, height - scale.Bottom);
                rects[7] = new Rectangle(scale.X, scale.Bottom, scale.Width, height - scale.Bottom);
                rects[8] = new Rectangle(scale.Right, scale.Bottom, width - scale.Right, height - scale.Bottom);

                for (Int32 idx = 0; idx < 9; ++idx)
                {
                    rects[idx].X += source.X;
                    rects[idx].Y += source.Y;
                }

                return new NinePatchImage()
                {
                    _texture = texture,
                    _rectangles = rects,
                    Width = width,
                    Height = height,
                    _drawOnlyInside = rects[4] == source
                };
            }
            else
            {
                Texture2D texture = ContentLoader.Current.Load<Texture2D>(name);

                Rectangle source = new Rectangle(0, 0, texture.Width, texture.Height);

                Int32 leftRight = (source.Width+1) / 2 - 1;
                Int32 topBottom = (source.Height+1) / 2 - 1;

                if (source.Width < 3)
                {
                    leftRight = 0;
                }

                if (source.Height < 3)
                {
                    topBottom = 0;
                }

                Rectangle[] rects = new Rectangle[9];

                rects[0] = new Rectangle(0, 0, leftRight, topBottom);
                rects[1] = new Rectangle(leftRight, 0, 1, topBottom);
                rects[2] = new Rectangle(leftRight + 1, 0, leftRight, topBottom);

                rects[3] = new Rectangle(0, topBottom, leftRight, 1);
                rects[4] = new Rectangle(leftRight, topBottom, 1, 1);
                rects[5] = new Rectangle(leftRight + 1, topBottom, leftRight, 1);

                rects[6] = new Rectangle(0, topBottom + 1, leftRight, topBottom);
                rects[7] = new Rectangle(leftRight, topBottom + 1, 1, topBottom);
                rects[8] = new Rectangle(leftRight + 1, topBottom + 1, leftRight, topBottom);

                return new NinePatchImage()
                {
                    _texture = texture,
                    _rectangles = rects,
                    Width = source.Width,
                    Height = source.Height,
                    _drawOnlyInside = (topBottom == 0 && leftRight == 0)
                };
            }
        }

        public Rectangle Draw(SpriteBatch spriteBatch, Rectangle destinationRect, Vector2 scale, Color color)
        {
            if (_drawOnlyInside)
            {
                Vector2 p4 = new Vector2(destinationRect.X, destinationRect.Y);
                Vector2 s4 = new Vector2((Single)destinationRect.Width / (Single)_rectangles[4].Width,
                                         (Single)destinationRect.Height / (Single)_rectangles[4].Height);

                spriteBatch.Draw(_texture, p4, _rectangles[4], color, 0, Vector2.Zero, s4, SpriteEffects.None, 0);

                return new Rectangle(0,0,destinationRect.Width, destinationRect.Height);
            }
            else
            {
                Int32 top = (Int32)(scale.Y * _rectangles[0].Height);
                Int32 bottom = (Int32)(scale.Y * _rectangles[6].Height);
                Int32 left = (Int32)(scale.X * _rectangles[0].Width);
                Int32 right = (Int32)(scale.X * _rectangles[2].Width);

                Int32 width = destinationRect.Width - left - right;
                Int32 height = destinationRect.Height - top - bottom;

                Single scaleLeft = (Single)left / (Single)_rectangles[0].Width;
                Single scaleRight = (Single)right / (Single)_rectangles[2].Width;

                Single scaleTop = (Single)top / (Single)_rectangles[0].Height;
                Single scaleBottom = (Single)bottom / (Single)_rectangles[6].Height;

                Single scaleWidth = (Single)width / (Single)_rectangles[4].Width;
                Single scaleHeight = (Single)height / (Single)_rectangles[4].Height;

                Vector2 p0 = new Vector2(destinationRect.X, destinationRect.Y);
                Vector2 s0 = new Vector2(scaleLeft, scaleTop);

                Vector2 p1 = new Vector2(destinationRect.X + left, destinationRect.Y);
                Vector2 s1 = new Vector2(scaleWidth, scaleTop);

                Vector2 p2 = new Vector2(destinationRect.X + left + width, destinationRect.Y);
                Vector2 s2 = new Vector2(scaleRight, scaleTop);

                Vector2 p3 = new Vector2(destinationRect.X, destinationRect.Y + top);
                Vector2 s3 = new Vector2(scaleLeft, scaleHeight);

                Vector2 p4 = new Vector2(destinationRect.X + left, destinationRect.Y + top);
                Vector2 s4 = new Vector2(scaleWidth, scaleHeight);

                Vector2 p5 = new Vector2(destinationRect.X + left + width, destinationRect.Y + top);
                Vector2 s5 = new Vector2(scaleRight, scaleHeight);

                Vector2 p6 = new Vector2(destinationRect.X, destinationRect.Y + top + height);
                Vector2 s6 = new Vector2(scaleLeft, scaleBottom);

                Vector2 p7 = new Vector2(destinationRect.X + left, destinationRect.Y + top + height);
                Vector2 s7 = new Vector2(scaleWidth, scaleBottom);

                Vector2 p8 = new Vector2(destinationRect.X + left + width, destinationRect.Y + top + height);
                Vector2 s8 = new Vector2(scaleRight, scaleBottom);

                spriteBatch.Draw(_texture, p0, _rectangles[0], color, 0, Vector2.Zero, s0, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p1, _rectangles[1], color, 0, Vector2.Zero, s1, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p2, _rectangles[2], color, 0, Vector2.Zero, s2, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p3, _rectangles[3], color, 0, Vector2.Zero, s3, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p4, _rectangles[4], color, 0, Vector2.Zero, s4, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p5, _rectangles[5], color, 0, Vector2.Zero, s5, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p6, _rectangles[6], color, 0, Vector2.Zero, s6, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p7, _rectangles[7], color, 0, Vector2.Zero, s7, SpriteEffects.None, 0);
                spriteBatch.Draw(_texture, p8, _rectangles[8], color, 0, Vector2.Zero, s8, SpriteEffects.None, 0);

                return new Rectangle(left, top, width, height);
            }
        }
    }
}
