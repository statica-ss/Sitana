/// This file is a part of the EBATIANOS.ESSENTIALS class library.
/// (c)2013-2014 EBATIANO'S a.k.a. Sebastian Sejud. All rights reserved.
///
/// THIS SOURCE FILE IS THE PROPERTY OF EBATIANO'S A.K.A. SEBASTIAN SEJUD 
/// AND IS NOT TO BE RE-DISTRIBUTED BY ANY MEANS WHATSOEVER WITHOUT 
/// THE EXPRESSED WRITTEN CONSENT OF EBATIANO'S A.K.A. SEBASTIAN SEJUD.
///
/// THIS SOURCE CODE CAN ONLY BE USED UNDER THE TERMS AND CONDITIONS OUTLINED
/// IN THE EBATIANOS.ESSENTIALS LICENSE AGREEMENT. 
/// EBATIANO'S A.K.A. SEBASTIAN SEJUD GRANTS TO YOU (ONE SOFTWARE DEVELOPER) 
/// THE LIMITED RIGHT TO USE THIS SOFTWARE ON A SINGLE COMPUTER.
///
/// CONTACT INFORMATION:
/// contact@ebatianos.com
/// www.ebatianos.com/essentials-library
/// 
///---------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Ebatianos.Content;
using Ebatianos;

namespace Ebatianos.Content
{
    public static class SpriteOptimizer
    {
        public static void OptimizeSprites(GraphicsDevice device, String dir, String[] sprites, Int32 maxTextureWidth, Int32 maxTextureHeight, BlendState blendMode)
        {
            List<Sprite> spritesList = new List<Sprite>();

            foreach (var name in sprites)
            {
                String path = name.Trim();
                Sprite sprite = ContentLoader.Current.Load<Sprite>(path);
                spritesList.Add(sprite);
            }

            spritesList.Sort(CompareSprites);

            while (spritesList.Count > 0)
            {
                Int32 spriteIndex = 0;
                Int32 lineHeight = 0;
                Int32 height = 0;
                Int32 offsetY = 0;
                Int32 offsetX = 0;

                Boolean doContinue = true;

                for (; spriteIndex < spritesList.Count && doContinue; ++spriteIndex)
                {
                    Sprite sprite = spritesList[spriteIndex];
                    Int32 rows = sprite.SpriteSheet[0].Height / sprite.FrameSize.Y;
                    Int32 columns = sprite.Columns;

                    for (Int32 sheet = 0; sheet < sprite.SpriteSheet.Length && doContinue; ++sheet)
                    {
                        for (Int32 idx = 0; idx < rows * columns; ++idx)
                        {
                            if (offsetX + sprite.FrameSize.X > maxTextureWidth)
                            {
                                offsetX = 0;
                                offsetY += lineHeight + 6;

                                if (offsetY + sprite.FrameSize.Y > maxTextureHeight)
                                {
                                    doContinue = false;
                                    break;
                                }
                            }

                            lineHeight = Math.Max(lineHeight, sprite.FrameSize.Y);
                            offsetX += sprite.FrameSize.X + 6;
                            height = Math.Max(height, offsetY + sprite.FrameSize.Y + 6);
                        }
                    }
                }

                height = Math.Min(height, maxTextureHeight);

                RenderTarget2D texture = new RenderTarget2D(device, maxTextureWidth, height);

                if (texture.Format != SurfaceFormat.Color)
                {
                    texture.Dispose();
                    return;
                }

                Viewport viewport = device.Viewport;

                device.SetRenderTarget(texture);
                device.Viewport = new Viewport(0, 0, maxTextureWidth, height);

                device.Clear(Color.Transparent);

                Int32 remainingSpriteIndex = OptimizeSprites(device, texture, spritesList, maxTextureWidth, height, blendMode);

                spritesList.RemoveRange(0, remainingSpriteIndex);

                device.SetRenderTarget(null);
                device.Viewport = viewport;
            }
        }

        private static Int32 CompareSprites(Sprite s1, Sprite s2)
        {
            if (s1.FrameSize.X == s2.FrameSize.X)
            {
                return Math.Sign(s1.FrameSize.Y - s2.FrameSize.Y);
            }

            return Math.Sign(s1.FrameSize.X - s2.FrameSize.X);
        }

        private static Int32 OptimizeSprites(GraphicsDevice device, Texture2D texture, List<Sprite> sprites, Int32 textureWidth, Int32 textureHeight, BlendState blendMode)
        {
            Int32 lineHeight = 0;
            Int32 offsetY = 0;
            Int32 offsetX = 0;

            using (SpriteBatch spriteBatch = new SpriteBatch(device))
            {
                Int32 spriteIndex = 0;

                for (; spriteIndex < sprites.Count; ++spriteIndex)
                {
                    Sprite sprite = sprites[spriteIndex];
                    Int32 rows = sprite.SpriteSheet[0].Height / sprite.FrameSize.Y;
                    Int32 columns = sprite.Columns;

                    sprite.OptimizedFrames = new List<Point[]>();

                    for (Int32 sheet = 0; sheet < sprite.SpriteSheet.Length; ++sheet)
                    {
                        Point[] frames = new Point[rows * columns];

                        for (Int32 idx = 0; idx < rows * columns; ++idx)
                        {
                            if (offsetX + sprite.FrameSize.X > textureWidth)
                            {
                                offsetX = 0;
                                offsetY += lineHeight + 6;

                                if (offsetY + sprite.FrameSize.Y > textureHeight)
                                {
                                    sprite.OptimizedFrames = null;

                                    return spriteIndex;
                                }
                            }

                            Int32 offX = (idx % sprite.Columns) * sprite.FrameSize.X;
                            Int32 offY = (idx / sprite.Columns) * sprite.FrameSize.Y;

                            

                            spriteBatch.Begin();

                            Rectangle src = new Rectangle(offX, offY, sprite.FrameSize.X, sprite.FrameSize.Y);
                            Vector2 dest = new Vector2(offsetX, offsetY);

                            spriteBatch.Draw(sprite.SpriteSheet[sheet], dest, src, Color.White);

                            // Top line
                            src = new Rectangle(offX, offY, sprite.FrameSize.X, 1);
                            dest = new Vector2(offsetX, offsetY-1);

                            spriteBatch.Draw(sprite.SpriteSheet[sheet], dest, src, Color.White);

                            // Bottom line
                            src = new Rectangle(offX, offY+sprite.FrameSize.Y-1, sprite.FrameSize.X, 1);
                            dest = new Vector2(offsetX, offsetY + sprite.FrameSize.Y);

                            spriteBatch.Draw(sprite.SpriteSheet[sheet], dest, src, Color.White);

                            // Left line
                            src = new Rectangle(offX, offY, 1, sprite.FrameSize.Y);
                            dest = new Vector2(offsetX-1, offsetY);

                            spriteBatch.Draw(sprite.SpriteSheet[sheet], dest, src, Color.White);

                            // Right line
                            src = new Rectangle(offX+ sprite.FrameSize.X - 1, offY, 1, sprite.FrameSize.Y);
                            dest = new Vector2(offsetX + sprite.FrameSize.X, offsetY);
                            spriteBatch.Draw(sprite.SpriteSheet[sheet], dest, src, Color.White);

                            spriteBatch.End();

                            frames[idx] = new Point(offsetX, offsetY);

                            lineHeight = Math.Max(lineHeight, sprite.FrameSize.Y);

                            offsetX += sprite.FrameSize.X + 6;
                        }

                        sprite.OptimizedFrames.Add(frames);
                    }

                    for (Int32 sheet = 0; sheet < sprite.SpriteSheet.Length; ++sheet)
                    {
                        sprite.SpriteSheet[sheet].Dispose();
                        sprite.SpriteSheet[sheet] = texture;
                    }
                }

                return spriteIndex;
            }
        }
    }
}

