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

        public void DrawText(UniversalFont font, StringBuilder text, Rectangle target, TextAlign align, Color color)
        {
            DrawText(font, text, target, align, color, 1);
        }

        public void DrawText(UniversalFont font, StringBuilder text, Rectangle target, TextAlign align, Color color, float scale)
        {
            if (font == null || text == null)
            {
                return;
            }

            Font = font;

            Vector2 size = _font.MeasureString(text) * scale;
            Vector2 position = TextPosition(ref target, align, size);

            if (_font.SitanaFont != null)
            {
                PrimitiveBatchNeeded();
                _font.SitanaFont.Draw(_primitiveBatch, text, position, color, new Vector2(scale));
            }
            else
            {
                Vector2 origin = TextOrigin(align, size);
                position.X += origin.X * scale;

                SpriteBatchIsNeeded();
                _spriteBatch.DrawString(_font.SpriteFont, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
            }
        }

        public void DrawText(UniversalFont font, string text, Rectangle target, TextAlign align, Color color)
        {
            DrawText(font, text, target, align, color, 1);
        }

        public void DrawText(UniversalFont font, string text, Rectangle target, TextAlign align, Color color, float scale)
        {
            if (font == null || text == null)
            {
                return;
            }

            Font = font;

            Vector2 size = _font.MeasureString(text) * scale;
            Vector2 position = TextPosition(ref target, align, size);

            if (_font.SitanaFont != null)
            {
                PrimitiveBatchNeeded();
                _font.SitanaFont.Draw(_primitiveBatch, text, position, color, new Vector2(scale));
            }
            else
            {
                Vector2 origin = TextOrigin(align, size);
                position.X += origin.X * scale;

                SpriteBatchIsNeeded();
                _spriteBatch.DrawString(_font.SpriteFont, text, position, color, 0, origin, scale, SpriteEffects.None, 0);
            }
        }

        public void DrawText(UniversalFont font, SharedString text, Rectangle target, TextAlign align, Color color)
        {
            DrawText(font, text, target, align, color, 1);
        }

        public void DrawText(UniversalFont font, SharedString text, Rectangle target, TextAlign align, Color color, float scale)
        {
            lock (text)
            {
                DrawText(font, text.StringBuilder, target, align, color, scale);
            }
        }

        public void DrawText(UniversalFont font, StringBuilder text, Point position, TextAlign align, Color color)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);
            DrawText(font, text, target, align, color);
        }

        public void DrawText(UniversalFont font, string text, Point position, TextAlign align, Color color)
        {
            DrawText(font, text, position, align, color, 1);
        }

        public void DrawText(UniversalFont font, string text, Point position, TextAlign align, Color color, float scale)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);
            DrawText(font, text, target, align, color, scale);
        }

        public void DrawText(UniversalFont font, SharedString text, Point position, TextAlign align, Color color)
        {
            DrawText(font, text, position, align, color, 1);
        }

        public void DrawText(UniversalFont font, SharedString text, Point position, TextAlign align, Color color, float scale)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);

            lock (text)
            {
                DrawText(font, text.StringBuilder, target, align, color, scale);
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

                Point tsize = new Point((int)((float)size.X / scale), (int)((float)size.Y / scale));

                PushVertex(new Vector2(position.X, position.Y), color, new Point(textureSrc.X, textureSrc.Y));
                PushVertex(new Vector2(position.X + size.X, position.Y), color, new Point(textureSrc.X + tsize.X, textureSrc.Y));
                PushVertex(new Vector2(position.X, position.Y + size.Y), color, new Point(textureSrc.X, textureSrc.Y + tsize.Y));

                PushVertex(new Vector2(position.X + size.X, position.Y), color, new Point(textureSrc.X + tsize.X, textureSrc.Y));
                PushVertex(new Vector2(position.X, position.Y + size.Y), color, new Point(textureSrc.X, textureSrc.Y + tsize.Y));
                PushVertex(new Vector2(position.X + size.X, position.Y + size.Y), color, new Point(textureSrc.X + tsize.X, textureSrc.Y + tsize.Y));
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
    }
}