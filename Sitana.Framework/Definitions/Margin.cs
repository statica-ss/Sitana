using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sitana.Framework
{
    public struct Margin
    {
        public static readonly Margin Zero = new Margin(0,0,0,0);

        public int Top;
        public int Bottom;
        public int Left;
        public int Right;

        public int Width
        {
            get
            {
                return Left + Right;
            }
        }

        public int Height
        {
            get
            {
                return Top + Bottom;
            }
        }

        public Margin(int all) : this(all, all, all, all) { }

        public Margin(int left, int top, int right, int bottom)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public Rectangle ComputeRect(Rectangle rect)
        {
            rect.X += Left;
            rect.Y += Top;
            rect.Width -= Width;
            rect.Height -= Height;

            return rect;
        }
    }
}
