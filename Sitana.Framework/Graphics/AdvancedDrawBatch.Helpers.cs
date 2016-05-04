// SITANA - Copyright (C) The Sitana Team.
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;

namespace Sitana.Framework.Graphics
{
    public partial class AdvancedDrawBatch
    {
        private Vector2 TextPosition(ref Rectangle target, TextAlign align, Vector2 size, TextRotation rotation)
        {
            if (rotation == TextRotation.Rotate270 || rotation == TextRotation.Rotate90)
            {
                size = new Vector2(size.Y, size.X);
            }

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

            switch(rotation)
            {
                case TextRotation.Rotate90:
                    pos.Y += size.Y;
                    break;

                case TextRotation.Rotate270:
                    pos.X += size.X;
                    break;

                case TextRotation.Rotate180:
                    pos.X += size.X;
                    pos.Y += size.Y;
                    break;
            }

            return pos;
        }

        private Vector2 TextOrigin(TextAlign align, Vector2 size)
        {
            Vector2 origin = Vector2.Zero;

            switch (align & TextAlign.Horz)
            {
            case TextAlign.Right:
                origin.X = size.X;
                break;

            case TextAlign.Center:
                origin.X = size.X / 2;
                break;
            }

            return origin;
        }
    }
}