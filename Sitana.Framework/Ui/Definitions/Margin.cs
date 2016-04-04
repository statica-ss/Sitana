// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Sitana.Framework.Ui.Core;
using Sitana.Framework.Ui.Definitions;

namespace Sitana.Framework.Ui
{
    public struct Margin: IMixable
    {
        public static readonly Margin None = new Margin(null);

        public int Top { get { return Get(_top).GetValueOrDefault(); } }
        public int Bottom { get { return Get(_bottom).GetValueOrDefault(); } }
        public int Left { get { return Get(_left).GetValueOrDefault(); } }
        public int Right { get { return Get(_right).GetValueOrDefault(); } }

        public Length? _top;
        public Length? _bottom;
        public Length? _left;
        public Length? _right;

        bool IMixable.IsMixMeaningful
        { 
            get
            {
                return !(_top.HasValue && _bottom.HasValue && _left.HasValue && _right.HasValue);
            }
        }

        IMixable IMixable.Mix(IMixable lessImportant)
        {
            if( !(lessImportant is Margin))
            {
                throw new Exception("Cannot mix different types.");
            }

            Margin margin = new Margin();

            Margin lessImportantMargin = (Margin)lessImportant;

            margin._top = _top ?? lessImportantMargin._top;
            margin._bottom = _bottom ?? lessImportantMargin._bottom;
            margin._left = _left ?? lessImportantMargin._left;
            margin._right = _right ?? lessImportantMargin._right;

            return margin;
        }

        public int Width
        {
            get
            {
                return Get(_left).GetValueOrDefault() + Get(_right).GetValueOrDefault();
            }
        }

        public int Height
        {
            get
            {
                return Get(_top).GetValueOrDefault() + Get(_bottom).GetValueOrDefault();
            }
        }

        public Margin(Length? all) : this(all, all, all, all) { }

        public Margin(double? left, double? top, double? right, double? bottom)
        {
            _top = top.HasValue ? new Length(top.Value) : (Length?)null;
            _bottom = bottom.HasValue ? new Length(bottom.Value) : (Length?)null;
            _left = left.HasValue ? new Length(left.Value) : (Length?)null;
            _right = right.HasValue ? new Length(right.Value) : (Length?)null;
        }

        public Margin(Length? left, Length? top, Length? right, Length? bottom)
        {
            _top = top;
            _bottom = bottom;
            _left = left;
            _right = right;
        }

        public Rectangle ComputeRect(Rectangle rect)
        {
            rect.X += Get(_left).GetValueOrDefault();
            rect.Y += Get(_top).GetValueOrDefault();
            rect.Width -= Width;
            rect.Height -= Height;

            return rect;
        }

        int? Get(Length? value)
        {
            if (value.HasValue)
            {
                return value.Value.Compute();
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
