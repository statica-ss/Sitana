using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitana.Framework.Content;

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

        public void DrawNinePatchRect(Rectangle target, Color color)
        {
            SpriteBatchIsNeeded();
            _ninePatchImage.Draw(_spriteBatch, target, Vector2.One, color);
        }
    }
}