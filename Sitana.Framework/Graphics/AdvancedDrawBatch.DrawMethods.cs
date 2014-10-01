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
            PrimitiveType = PrimitiveType.LineList;

            PushVertex(p1.ToVector2(), color);
            PushVertex(p2.ToVector2(), color);
        }

        public void DrawPolyline(Point[] points, Color color)
        {
            PrimitiveType = PrimitiveType.LineStrip;

            foreach (var p in points)
            {
                PushVertex(p.ToVector2(), color);
            }
        }

        public void DrawPolyline(Color color, params Point[] points)
        {
            DrawPolyline(points, color);
        }

        public void DrawPoint(Point p, Color color)
        {
            PrimitiveType = PrimitiveType.LineList;

            PushVertex(p.ToVector2(), color);
            PushVertex(new Vector2((int)p.X+1, (int)p.Y), color);
        }

        public void DrawText(StringBuilder text, Rectangle target, TextAlign align, Color color)
        {
            if (_font == null || text == null)
            {
                return;
            }

            SpriteBatchIsNeeded();

            Vector2 size = _font.MeasureString(text);
            Vector2 position = TextPosition(ref target, align, size);

            _spriteBatch.DrawString(_font, text, position, color);
        }

        public void DrawText(string text, Rectangle target, TextAlign align, Color color)
        {
            if (_font == null || text == null)
            {
                return;
            }

            SpriteBatchIsNeeded();

            Vector2 size = _font.MeasureString(text);
            Vector2 position = TextPosition(ref target, align, size);

            _spriteBatch.DrawString(_font, text, position, color);
        }

        public void DrawText(SharedString text, Rectangle target, TextAlign align, Color color)
        {
            if (_font == null || text == null)
            {
                return;
            }

            lock (text)
            {
                DrawText(text.StringBuilder, target, align, color);
            }
        }

        public void DrawText(StringBuilder text, Point position, TextAlign align, Color color)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);
            DrawText(text, target, align, color);
        }

        public void DrawText(string text, Point position, TextAlign align, Color color)
        {
            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);
            DrawText(text, target, align, color);
        }

        public void DrawText(SharedString text, Point position, TextAlign align, Color color)
        {
            if (_font == null || text == null)
            {
                return;
            }

            Rectangle target = new Rectangle(position.X, position.Y, 0, 0);

            lock (text)
            {
                DrawText(text.StringBuilder, target, align, color);
            }
        }

        public void DrawNinePatchRect(Rectangle target, Color color)
        {
            SpriteBatchIsNeeded();
            _ninePatchImage.Draw(_spriteBatch, target, Vector2.One, color);
        }

        public void DrawImage(Point position, Point size, Point textureSrc, Color color)
        {
            if (_texture != null)
            {
                PushVertex(new Vector2(position.X, position.Y), color, new Point(textureSrc.X, textureSrc.Y));
                PushVertex(new Vector2(position.X + size.X, position.Y), color, new Point(textureSrc.X + size.X, textureSrc.Y));
                PushVertex(new Vector2(position.X, position.Y + size.Y), color, new Point(textureSrc.X, textureSrc.Y+size.Y));
                
                PushVertex(new Vector2(position.X + size.X, position.Y), color, new Point(textureSrc.X + size.X, textureSrc.Y));
                PushVertex(new Vector2(position.X, position.Y + size.Y), color, new Point(textureSrc.X, textureSrc.Y + size.Y));
                PushVertex(new Vector2(position.X+size.X, position.Y+size.Y), color, new Point(textureSrc.X+size.X, textureSrc.Y+size.Y));
            }
            else
            {
                DrawRectangle(new Rectangle(position.X, position.Y, size.X, size.Y), color);
            }
        }

        public void DrawImage(Rectangle target, Rectangle textureSrc, Color color)
        {
            if (_texture != null)
            {
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