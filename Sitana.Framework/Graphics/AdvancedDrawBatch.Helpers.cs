// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitana.Framework.Graphics
{
    public partial class AdvancedDrawBatch
    {
        private Vector2 TextPosition(ref Rectangle target, TextAlign align, Vector2 size)
        {
            Vector2 pos = target.Location.ToVector2();

            switch (align & TextAlign.Horz)
            {
                case TextAlign.Right:
                    pos.X = target.Right - (int)size.X;
                    break;

                case TextAlign.Center:
                    pos.X = target.Center.X - (int)size.X / 2;
                    break;
            }

            switch (align & TextAlign.Vert)
            {
                case TextAlign.Bottom:
                    pos.Y = target.Bottom - (int)size.Y;
                    break;

                case TextAlign.Middle:
                    pos.Y = target.Center.Y - (int)size.Y / 2;
                    break;
            }

            return pos;
        }
    }
}