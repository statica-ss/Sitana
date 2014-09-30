// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Core;

namespace Sitana.Framework
{
    public struct Margin
    {
        public static readonly Margin None = new Margin(null);

        public int Top { get { return Get(_top).GetValueOrDefault(); } }
        public int Bottom { get { return Get(_bottom).GetValueOrDefault(); } }
        public int Left { get { return Get(_left).GetValueOrDefault(); } }
        public int Right { get { return Get(_right).GetValueOrDefault(); } }

        public int? _top;
        public int? _bottom;
        public int? _left;
        public int? _right;

        public int Width
        {
            get
            {
                return _left.GetValueOrDefault() + _right.GetValueOrDefault();
            }
        }

        public int Height
        {
            get
            {
                return _top.GetValueOrDefault() + _bottom.GetValueOrDefault();
            }
        }

        public Margin(int? all) : this(all, all, all, all) { }

        public Margin(int? left, int? top, int? right, int? bottom)
        {
            _top = top;
            _bottom = bottom;
            _left = left;
            _right = right;
        }

        public Rectangle ComputeRect(Rectangle rect)
        {
            rect.X += _left.GetValueOrDefault();
            rect.Y += _top.GetValueOrDefault();
            rect.Width -= Width;
            rect.Height -= Height;

            return rect;
        }

        int? Get(int? value)
        {
            if (value.HasValue)
            {
                return (int)(UiUnit.Unit*value.Value);
            }

            return null;
        }

        public void RepairRect(ref Rectangle rect, int width, int height)
        {
            if (_left.HasValue && rect.Left < Get(_left).Value)
            {
                int diff = Get(_left).Value - rect.Left;
                rect.X += diff;
            }

            if (_right.HasValue && rect.Right > width - Get(_right).Value)
            {
                int diff = rect.Right - (width - Get(_right).Value);
                rect.X -= diff;
            }

            if (_left.HasValue && rect.Left < Get(_left).Value)
            {
                int diff = Get(_left).Value - rect.Left;
                rect.X += diff;
                rect.Width -= diff;
            }

            if (_top.HasValue && rect.Top < Get(_top).Value)
            {
                int diff = Get(_top).Value - rect.Top;
                rect.Y += diff;
            }

            if (_bottom.HasValue && rect.Bottom > height - Get(_bottom).Value)
            {
                int diff = rect.Bottom - (height - Get(_bottom).Value);
                rect.Y -= diff;
            }

            if (_top.HasValue && rect.Top < Get(_top).Value)
            {
                int diff = Get(_top).Value - rect.Top;
                rect.Y += diff;
                rect.Height -= diff;
            }
        }
    }
}
