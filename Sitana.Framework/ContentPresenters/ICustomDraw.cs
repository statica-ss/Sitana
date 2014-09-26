// SITANA - Copyright (C) The Sitana Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sitana.Framework.Content
{
    public interface ICustomDraw
    {
        void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle source, Color color, Single angle, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects);
    }
}
