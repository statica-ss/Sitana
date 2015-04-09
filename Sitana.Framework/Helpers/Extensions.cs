// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sitana.Framework
{
    public static class Extensions
    {
        public static Point ToPoint(this Vector2 vec)
        {
            return new Point((int)vec.X, (int)vec.Y);
        }

        public static Vector2 ToVector2(this Point pt)
        {
            return new Vector2(pt.X, pt.Y);
        }

        public static Vector2 TrimToIntValues(this Vector2 vec)
        {
            return new Vector2((int)vec.X, (int)vec.Y);
        }

        public static int ToInt(this Color color)
        {
            return (color.A << 24) | (color.R << 16) | (color.G << 8) | color.B;
        }

        public static Color ToColor(this int color)
        {
            int a = (color >> 24) & 0xff;
            int r = (color >> 16) & 0xff;
            int g = (color >> 8) & 0xff;
            int b = (color) & 0xff;

            return Color.FromNonPremultiplied(r,g,b,a);
        }

        public static Vector2 ToVector2(this MouseState ms)
        {
            return new Vector2(ms.X, ms.Y);
        }

        public static bool Contains(this Rectangle rect, Vector2 vec)
        {
            return rect.Contains(vec.ToPoint());
        }
    }
}
