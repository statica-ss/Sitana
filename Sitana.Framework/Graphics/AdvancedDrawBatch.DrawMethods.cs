// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content;
using Sitana.Framework.Cs;
using Sitana.Framework;

namespace Sitana.Framework.Graphics
{
    public partial class AdvancedDrawBatch
    {
        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            Texture = null;
            PrimitiveType = PrimitiveType.TriangleList;

            PushVertex(new Vector2(rectangle.Left, rectangle.Top), color);
            PushVertex(new Vector2(rectangle.Left, rectangle.Bottom), color);
            PushVertex(new Vector2(rectangle.Right, rectangle.Top), color);

            PushVertex(new Vector2(rectangle.Left, rectangle.Bottom), color);
            PushVertex(new Vector2(rectangle.Right, rectangle.Top), color);
            PushVertex(new Vector2(rectangle.Right, rectangle.Bottom), color);
        }

        public void DrawLine(Point p1, Point p2, Color color)
        {
            Texture = null;

            PrimitiveType = PrimitiveType.LineList;

            PushVertex(p1.ToVector2(), color);
            PushVertex(p2.ToVector2(), color);
        }

        public void DrawPolyline(Point[] points, Color color)
        {
            Texture = null;

            PrimitiveType = PrimitiveType.LineStrip;

            foreach (var p in points)
            {
                PushVertex(p.ToVector2(), color);
            }
        }

        public void DrawPolyline(Color color, params Point[] points)
        {
            Texture = null;

            DrawPolyline(points, color);
        }

        public void DrawPoint(Point p, Color color)
        {
            Texture = null;

            PrimitiveType = PrimitiveType.LineList;

            PushVertex(p.ToVector2(), color);
            PushVertex(new Vector2((int)p.X+1, (int)p.Y), color);
        }

        public Vector2 DrawText(UniversalFont font, StringBuilder text, Rectangle target, TextAlign align, Color color)
        {
            return DrawText(font, text, target, align, color, 0, 0, 1);
        }

        public Vector2 DrawText(UniversalFont font, StringBuilder text, Rectangle target, TextAlign align, Color[] colors, float opacity)
        {
            return DrawText(font, text, target, align, colors, opacity, 0, 0, 1);
        }

        public Vector2 DrawText(UniversalFont font, StringBuilder text, Rectangle target, TextAlign align, Color[] colors, float opacity, float spacing, float lineHeight, float scale)
        {
            if (font == null || text == null)
            {
                return Vector2.Zero;
            }

            Font = font;

            Vector2 size = _font.MeasureString(text, spacing, lineHeight) * scale;
            Vector2 position = TextPosition(ref target, align, size);

            if (_font.SitanaFont != null)
            {
                PrimitiveType = PrimitiveType.TriangleList;

                PrimitiveBatchNeeded();
                _font.SitanaFont.Draw(_primitiveBatch, text, position, colors, opacity, spacing, lineHeight, new Vector2(scale));
            }
            else
            {
                throw new Exception("Only Sitana Font supports colored text.");
            }

            return size;
        }

        public Vector2 DrawText(UniversalFont font, StringBuilder text, Rectangle target, TextAlign align, Color color, float spacing, float lineHeight, float scale)
        {
            if (font == null || text == null)
            {
                return Vector2.Zero;
            }

            Font = font;

            Vector2 size = _font.MeasureString(text, spacing, lineHeight) * scale;
            Vector2 position = TextPosition(ref target, align, size);

            if (_font.SitanaFont != null)
            {
                PrimitiveType = PrimitiveType.TriangleList;

                PrimitiveBatchNeeded();
                _font.SitanaFont.Draw(_primitiveBatch, text, position, color, spacing, lineHeight, new Vector2(scale));
            }
            else
            {
                Vector2 origin = TextOrigin(align, size);
                position.X += origin.X * scale;

                SpriteBatchIsNeeded();
                _spriteBatch.DrawString(_font.SpriteFont, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
            }

            return size;
        }

        public Vector2 DrawText(UniversalFont font, string text, Rectangle target, TextAlign align, Color color)
        {
            return DrawText(font, text, target, align, color, 0, 0, 1);
        }

        public Vector2 DrawText(UniversalFont font, string text, Rectangle target, TextAlign align, Color color, float spacing, float lineHeight, float scale)
        {
            if (font == null || text == null)
            {
                return Vector2.Zero;
            }

            Font = font;

            Vector2 position;
            Vector2 size;

            if(align != TextAlign.None )
            {
                size = _font.MeasureString(text, spacing, lineHeight) * scale;
                position = TextPosition(ref target, align, size);
            }
            else
            {
                size = Vector2.Zero;
                position = target.Location.ToVector2();
            }

            if (_font.SitanaFont != null)
            {
                PrimitiveType = PrimitiveType.TriangleList;

                PrimitiveBatchNeeded();
                _font.SitanaFont.Draw(_primitiveBatch, text, position, color, spacing, lineHeight, new Vector2(scale));
            }
            else
            {
                Vector2 origin = Vector2.Zero;

                if (align != TextAlign.None)
                {
                    origin = TextOrigin(align, size);
                }

                position.X += origin.X * scale;

                SpriteBatchIsNeeded();
                _spriteBatch.DrawString(_font.SpriteFont, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
            }

            return size;
        }

        public Vector2 DrawText(UniversalFont font, SharedString text, Rectangle target, TextAlign align, Color color)
        {
            return DrawText(font, text, target, align, color, 0, 0, 1);
        }

        public Vector2 DrawText(UniversalFont font, SharedString text, Rectangle target, TextAlign align, Color color, float spacing, float lineHeight, float scale)
        {
            lock (text)
            {
                return DrawText(font, text.StringBuilder, target, align, color, spacing, lineHeight, scale);
            }
        }

        public Vector2 DrawText(UniversalFont font, StringBuilder text, Point position, TextAlign align, Color color)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);
            return DrawText(font, text, target, align, color);
        }

        public Vector2 DrawText(UniversalFont font, string text, Point position, TextAlign align, Color color)
        {
            return DrawText(font, text, position, align, color, 0, 0, 1);
        }

        public Vector2 DrawText(UniversalFont font, string text, Point position, TextAlign align, Color color, float spacing, float lineHeight, float scale)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);
            return DrawText(font, text, target, align, color, spacing, lineHeight, scale);
        }

        public Vector2 DrawText(UniversalFont font, SharedString text, Point position, TextAlign align, Color color)
        {
            return DrawText(font, text, position, align, color, 0, 0, 1);
        }

        public Vector2 DrawText(UniversalFont font, SharedString text, Point position, TextAlign align, Color color, float spacing, float lineHeight, float scale)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);

            lock (text)
            {
                return DrawText(font, text.StringBuilder, target, align, color, spacing, lineHeight, scale);
            }
        }

        public void DrawNinePatchRect(NinePatchImage image, Rectangle target, Color color)
        {
            DrawNinePatchRect(image, target, color, 1);
        }

        public void DrawNinePatchRect(NinePatchImage image, Rectangle target, Color color, float scale)
        {
            if (image != null)
            {
                Texture = image.Texture;
                SpriteBatchIsNeeded();

                image.Draw(_spriteBatch, target, new Vector2(scale), color);
            }
            else
            {
                DrawRectangle(target, color);
            }
        }

        public void DrawImage(Texture2D texture, Point position, Point size, Point textureSrc, Color color)
        {
            DrawImage(texture, position, size, textureSrc, 1, color);
        }

        public void DrawImage(Texture2D texture, Point position, Point size, Point textureSrc, float scale, Color color)
        {
            if (texture != null)
            {
                Texture = texture;
                PrimitiveType = PrimitiveType.TriangleList;

				Vector2 tsizeTl = new Vector2((float)textureSrc.X / (float)texture.Width, (float)textureSrc.Y / (float)texture.Height);
				Vector2 tsizeBr = new Vector2((float)(textureSrc.X + (float)size.X / scale) / (float)texture.Width, (float)(textureSrc.Y + (float)size.Y / scale) / (float)texture.Height);

				PushVertex(new Vector2(position.X, position.Y), color, new Vector2(tsizeTl.X, tsizeTl.Y));
				PushVertex(new Vector2(position.X + size.X, position.Y), color, new Vector2(tsizeBr.X, tsizeTl.Y));
				PushVertex(new Vector2(position.X, position.Y + size.Y), color, new Vector2(tsizeTl.X, tsizeBr.Y));

				PushVertex(new Vector2(position.X + size.X, position.Y), color, new Vector2(tsizeBr.X, tsizeTl.Y));
				PushVertex(new Vector2(position.X, position.Y + size.Y), color, new Vector2(tsizeTl.X, tsizeBr.Y));
				PushVertex(new Vector2(position.X + size.X, position.Y + size.Y), color, new Vector2(tsizeBr.X, tsizeBr.Y));
            }
            else
            {
                DrawRectangle(new Rectangle(position.X, position.Y, size.X, size.Y), color);
            }
        }

        public void DrawImage(Texture2D texture, Rectangle target, Rectangle textureSrc, Color color)
        {
            if (texture != null)
            {
                Texture = texture;
                PrimitiveType = PrimitiveType.TriangleList;

                PushVertex(new Vector2(target.Left, target.Top), color, new Point(textureSrc.Left, textureSrc.Top));
                PushVertex(new Vector2(target.Right, target.Top), color, new Point(textureSrc.Right, textureSrc.Top));
                PushVertex(new Vector2(target.Left, target.Bottom), color, new Point(textureSrc.Left, textureSrc.Bottom));

                PushVertex(new Vector2(target.Right, target.Top), color, new Point(textureSrc.Right, textureSrc.Top));
                PushVertex(new Vector2(target.Left, target.Bottom), color, new Point(textureSrc.Left, textureSrc.Bottom));
                PushVertex(new Vector2(target.Right, target.Bottom), color, new Point(textureSrc.Right, textureSrc.Bottom));
            }
            else
            {
                DrawRectangle(target, color);
            }
        }

        public void DrawImage(Texture2D texture, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, float scale)
        {
            if (texture != null)
            {
                SpriteBatchIsNeeded();
                _spriteBatch.Draw(texture, position, source, color, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public void DrawTextureLine(Point point1, Point point2, Color color, float width)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            Vector2 vec = new Vector2(point2.X - point1.X, point2.Y - point1.Y);
            float length = vec.Length();

            if (OnePixelWhiteTexture != null)
            {
                SpriteBatchIsNeeded();
                _spriteBatch.Draw(OnePixelWhiteTexture, point1.ToVector2(), null, color, angle, Vector2.Zero, new Vector2(length, width), SpriteEffects.None, 0);
            }
        }
    }
}