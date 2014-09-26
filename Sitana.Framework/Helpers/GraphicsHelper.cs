// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sitana.Framework.Content;
using System;

namespace Sitana.Framework
{
    /// <summary>
    /// Helper class for Graphics User Interface.
    /// </summary>
    public static class GraphicsHelper
    {
        /// <summary>
        /// Creates Point object from Vector2.
        /// </summary>
        /// <param name="vector">Input vector.</param>
        /// <returns>Point with coordinates from vector.</returns>
        public static Point PointFromVector2(Vector2 vector)
        {
            return new Point((Int32)vector.X, (Int32)vector.Y);
        }

        /// <summary>
        /// Creates Vector2 object from Point.
        /// </summary>
        /// <param name="point">Input point.</param>
        /// <returns>Vector2 with coordinates from point.</returns>
        public static Vector2 Vector2FromPoint(Point point)
        {
            return new Vector2((Single)point.X, (Single)point.Y);
        }

        /// <summary>
        /// Returns size of a texture.
        /// </summary>
        /// <param name="texture">Texture to get the size of.</param>
        /// <returns>Size of the texture.</returns>
        public static Point TextureSize(Texture2D texture)
        {
            return new Point(texture.Width, texture.Height);
        }

        /// <summary>
        /// Draws a line on screen.
        /// </summary>
        /// <param name="batch">SpriteBatch</param>
        /// <param name="blank">Texture 1x1 with white</param>
        /// <param name="width">Width of line</param>
        /// <param name="color">Color of line</param>
        /// <param name="point1">Point 1</param>
        /// <param name="point2">Point 2</param>
        public static void DrawLine(SpriteBatch batch, Texture2D blank,
                float width, Color color, Vector2 point1, Vector2 point2)
        {
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = Vector2.Distance(point1, point2);

            batch.Draw(blank, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, width),
                       SpriteEffects.None, 0);
        }

        /// <summary>
        /// Calculates the scale.
        /// </summary>
        /// <returns>The scale.</returns>
        /// <param name="texture">Texture.</param>
        /// <param name="desiredSize">Desired size.</param>
        public static Vector2 CalculateScale(Texture2D texture, Vector2 desiredSize)
        {
            return new Vector2(desiredSize.X / (Single)texture.Width, desiredSize.Y / (Single)texture.Height);
        }

        /// <summary>
        /// Calculates the scale.
        /// </summary>
        /// <returns>The scale.</returns>
        /// <param name="texture">Texture.</param>
        /// <param name="desiredSize">Desired size.</param>
        public static Vector2 CalculateScale(Texture2D texture, Point desiredSize)
        {
            return new Vector2((Single)desiredSize.X / (Single)texture.Width, (Single)desiredSize.Y / (Single)texture.Height);
        }

        public static Color MixColors(Color c1, Color c2, Single c2Mix)
        {
            Vector4 v1 = c1.ToVector4();
            Vector4 v2 = c2.ToVector4();

            Vector4 v = v1 * (1 - c2Mix) + v2 * c2Mix;

            return new Color(v);
        }

        public static Rectangle IntersectRectangle(ref Rectangle r1, ref Rectangle r2)
        {
            Int32 x1 = Math.Max(r1.X, r2.X);
            Int32 x2 = Math.Min(r1.Right, r2.Right);

            Int32 y1 = Math.Max(r1.Y, r2.Y);
            Int32 y2 = Math.Min(r1.Bottom, r2.Bottom);

            if (x1 > x2 || y1 > y2)
            {
                return Rectangle.Empty;
            }

            return new Rectangle(x1, y1, x2-x1, y2-y1);
        }
    }
}
