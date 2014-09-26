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

namespace Ebatianos.Platform
{
    /// <summary>
    /// Helper class to draw maps.
    /// </summary>
    public static class LayerPainter
    {
        /// <summary>
        /// Draws map's layer.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to render textures.</param>
        /// <param name="tileset">Texture of tileset for layer.</param>
        /// <param name="position">Position of the observer.</param>
        /// <param name="tileSize">Size of tiles.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="layer">Object of map's layer.</param>
        /// <param name="editMode">Defines if layer should be drawn in edit mode (for editors) - edit mode doesn't allow tiling.</param>
        /// <param name="areaSize">Size of destination area.</param>
        /// <param name="color">Color to multiply contents by.</param>
        public static Vector2 PaintLayer(SpriteBatch spriteBatch, Texture2D tileset, Vector2 position, Int32 tileSize, Single scale, Map.Layer layer, Boolean editMode, Point areaSize, Color color, Boolean snapPixels, Int32[,] optimizedLayer)
        {
            if (!editMode && !layer.IsMainLayer)
            {
                Single newScale = (Single)(Math.Pow(scale, layer.ScaleExponent) * layer.Scale);
                scale = newScale;
            }

            // Compute tile size for given scale.
            Single scaledTileSize = ((Single)tileSize * scale);

            if (snapPixels)
            {
                scaledTileSize = (Single)(Int32)scaledTileSize;
            }

            // Recalculate observer's position to pixels.
            position.X *= (Single)scaledTileSize;
            position.Y *= (Single)scaledTileSize;

            Vector2 offset = new Vector2(position.X, position.Y);

            // Compute indices at the begining of area.
            Int32 xStart = (Int32)(offset.X / scaledTileSize);
            Int32 yStart = (Int32)(offset.Y / scaledTileSize);

            // Compute indices at the and of area.
            Int32 xEnd = xStart + (Int32)(areaSize.X / scaledTileSize) + 3;
            Int32 yEnd = yStart + (Int32)(areaSize.Y / scaledTileSize) + 3;

            // Compute offset for tile.
            Single xOffset = offset.X % scaledTileSize;
            Single yOffset = offset.Y % scaledTileSize;

            Int32 moveX = Math.Min(9, xStart);
            Int32 moveY = Math.Min(9, yStart);

            xStart -= moveX;
            yStart -= moveY;

            if (snapPixels)
            {
                xOffset = (Single)(Int32)xOffset;
                yOffset = (Single)(Int32)yOffset;
            }

            // Helper variables containing current drawing position.
            Int32 posY = 0;
            Int32 posX = 0;

            // Array of tiles.
            //UInt16[,]   tiles       = layer.Tiles;

            // Compute scale for drawing. Scale for drawing is set to value, which ensures obe tile to be integer size.
            Single drawScale = scaledTileSize / (Single)tileSize;

            // Get layer width and height.
            Int32 layerWidth = layer.Width;
            Int32 layerHeight = layer.Height;

            // Set tiling flags.
            Boolean tiledWidth = layer.IsTiledWidth && !editMode;
            Boolean tiledHeight = layer.IsTiledHeight && !editMode;

            // If not tiling width, crop ending.
            if (!tiledWidth)
            {
                xEnd = Math.Min(xEnd, layerWidth);
            }

            // If not tiling height, crop ending.
            if (!tiledHeight)
            {
                yEnd = Math.Min(yEnd, layerHeight);
            }

            // If not tiling width and position is outside layer area, return without drawing anything.
            if (!tiledWidth)
            {
                if (xStart >= layerWidth)
                {
                    return Vector2.Zero;
                }
            }

            // If not tiling height and position is outside layer area, return without drawing anything.
            if (!tiledHeight)
            {
                if (yStart >= layerHeight)
                {
                    return Vector2.Zero;
                }
            }

            //-- Draw all tiles on screen.

            // Reset vertical position.
            posY = 0;

            if (optimizedLayer != null)
            {
                // Go thru all vertical indices.
                for (Int32 hIdx = yStart; hIdx < yEnd; ++hIdx)
                {
                    // Reset horizontal position.
                    posX = 0;

                    UInt16[,] tiles = layer.Tiles;

                    // Go thru horizontal indices.
                    for (Int32 wIdx = xStart; wIdx < xEnd; ++wIdx)
                    {
                        Int32 idx = (Int32)(wIdx % layerWidth);
                        Int32 idy = (Int32)(hIdx % layerHeight);

                        Int32 width = 1;
                        Int32 height = 1;

                        // Get tile at position.
                        Int32 tileOpt = optimizedLayer[idx, idy];

                        if (tileOpt != 0xffff)
                        {
                            UInt16 tile = 0;

                            if (tileOpt == 0)
                            {
                                tile = tiles[idx, idy];
                            }
                            else
                            {
                                tile = (UInt16)(tileOpt & 0xffff);

                                width = (tileOpt >> 16) & 0xff;
                                height = (tileOpt >> 24) & 0xff;
                            }

                            // Ensure it's not empty.
                            if (0 != tile && tile != 0xffff)
                            {
                                // Compute source position of tile.
                                Int32 srcXOffset = (tile & 0xff) * tileSize;
                                Int32 srcYOffset = (tile >> 8) * tileSize;

                                // Compute target position of tile.
                                Single targetX = (((Int32)posX - moveX) * scaledTileSize) - xOffset;
                                Single targetY = (((Int32)posY - moveY) * scaledTileSize) - yOffset;

                                if (targetX + width * scaledTileSize >= 0 && targetY + height * scaledTileSize >= 0)
                                {
                                    // Draw tile.
                                    spriteBatch.Draw(tileset, new Vector2(targetX, targetY),
                                                   new Rectangle(srcXOffset, srcYOffset, tileSize * width, tileSize * height), color, 0, new Vector2(0, 0), drawScale, SpriteEffects.None, 0);
                                }
                            }
                        }

                        // Increment horizontal position.
                        posX++;
                    }

                    // Increment vertical position.
                    ++posY;
                }
            }
            else
            {
                // Go thru all vertical indices.
                for (Int32 hIdx = yStart; hIdx < yEnd; ++hIdx)
                {
                    // Reset horizontal position.
                    posX = 0;

                    UInt16[,] tiles = layer.Tiles;

                    // Go thru horizontal indices.
                    for (Int32 wIdx = xStart; wIdx < xEnd; ++wIdx)
                    {
                        Int32 idx = (Int32)(wIdx % layerWidth);
                        Int32 idy = (Int32)(hIdx % layerHeight);

                        UInt16 tile = 0;

                        tile = tiles[idx, idy];

                        // Ensure it's not empty.
                        if (0 != tile && tile != 0xffff)
                        {
                            // Compute source position of tile.
                            Int32 srcXOffset = (tile & 0xff) * tileSize;
                            Int32 srcYOffset = (tile >> 8) * tileSize;

                            // Compute target position of tile.
                            Single targetX = (((Int32)posX - moveX) * scaledTileSize) - xOffset;
                            Single targetY = (((Int32)posY - moveY) * scaledTileSize) - yOffset;

                            // Draw tile.
                            spriteBatch.Draw(tileset, new Vector2(targetX, targetY),
                                           new Rectangle(srcXOffset, srcYOffset, tileSize, tileSize), color, 0, new Vector2(0, 0), drawScale, SpriteEffects.None, 0);
                        }

                        // Increment horizontal position.
                        posX++;
                    }

                    // Increment vertical position.
                    ++posY;
                }
            }

            Single endX = (((Int32)posX - moveX) * scaledTileSize) - xOffset;
            Single endY = (((Int32)posY - moveY) * scaledTileSize) - yOffset;

            return new Vector2(endX, endY);
        }

        public static Vector2 ComputePosition(Vector2 cameraPosition, Vector2 position, Int32 tileSize, Single scale)
        {
            // Compute tile size for given scale.
            Int32 scaledTileSize = (Int32)((Single)tileSize * scale);

            // Recalculate observer's position to pixels.
            cameraPosition.X *= (Single)scaledTileSize;
            cameraPosition.Y *= (Single)scaledTileSize;

            position.X *= (Single)scaledTileSize;
            position.Y *= (Single)scaledTileSize;

            return position - cameraPosition;
        }

        public static void PaintCollisions(SpriteBatch spriteBatch, Map.Layer layer, Boolean useMasks, Texture2D maskTexture, Color color, Vector2 position, Int32 tileSize, Single scale, Point areaSize)
        {
            // Compute tile size for given scale.
            Single scaledTileSize = ((Single)tileSize * scale);

            scaledTileSize = (Single)(Int32)scaledTileSize;

            // Recalculate observer's position to pixels.
            position.X *= (Single)scaledTileSize;
            position.Y *= (Single)scaledTileSize;

            Vector2 offset = new Vector2(position.X, position.Y);

            // Compute indices at the begining of area.
            Int32 xStart = (Int32)(offset.X / scaledTileSize);
            Int32 yStart = (Int32)(offset.Y / scaledTileSize);

            // Compute indices at the and of area.
            Int32 xEnd = xStart + (Int32)(areaSize.X / scaledTileSize) + 3;
            Int32 yEnd = yStart + (Int32)(areaSize.Y / scaledTileSize) + 3;

            // Compute offset for tile.
            Single xOffset = offset.X % scaledTileSize;
            Single yOffset = offset.Y % scaledTileSize;


            xOffset = (Single)(Int32)xOffset;
            yOffset = (Single)(Int32)yOffset;


            // Helper variables containing current drawing position.
            Int32 posY = 0;
            Int32 posX = 0;

            // Array of tiles.
            //UInt16[,] tiles = layer.Tiles;

            // Compute scale for drawing. Scale for drawing is set to value, which ensures obe tile to be integer size.
            //Single drawScale = scaledTileSize / (Single)tileSize;

            // Get layer width and height.
            Int32 layerWidth = layer.Width;
            Int32 layerHeight = layer.Height;

            xEnd = Math.Min(xEnd, layerWidth);
            yEnd = Math.Min(yEnd, layerHeight);

            if (xStart >= layerWidth)
            {
                return;
            }


            if (yStart >= layerHeight)
            {
                return;
            }

            //-- Draw all tiles on screen.

            // Reset vertical position.
            posY = 0;

            // Go thru all vertical indices.
            for (Int32 hIdx = yStart; hIdx < yEnd; ++hIdx)
            {
                // Reset horizontal position.
                posX = 0;

                // Go thru horizontal indices.
                for (Int32 wIdx = xStart; wIdx < xEnd; ++wIdx)
                {
                    // Get tile at position.
                    UInt16 tile = layer.Tiles[wIdx % layerWidth, hIdx % layerHeight];

                    // Ensure it's not empty.
                    if (0 != tile && 0xffff != tile)
                    {
                        // Compute target position of tile.
                        Single targetX = (posX * scaledTileSize) - xOffset;
                        Single targetY = (posY * scaledTileSize) - yOffset;

                        if (useMasks)
                        {
                            // Compute source position of tile.
                            //Int32 srcXOffset = (tile & 0xff) * tileSize;
                            //Int32 srcYOffset = (tile >> 8) * tileSize;
                        }
                        else
                        {
                            Single drawCollisionScale = (Single)scaledTileSize;
                            spriteBatch.Draw(maskTexture, new Vector2(targetX, targetY), null, color, 0, new Vector2(0, 0), drawCollisionScale, SpriteEffects.None, 0);
                        }

                        // Draw tile.

                    }

                    // Increment horizontal position.
                    posX++;
                }

                // Increment vertical position.
                ++posY;
            }


        }

        /// <summary>
        /// Paints layer events. It's used in editors for previewing layer events.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw textures.</param>
        /// <param name="events">Array of textures representing events.</param>
        /// <param name="eventMark">Texture with all white area to draw event mark.</param>
        /// <param name="position">Position of the observer.</param>
        /// <param name="tileSize">Size of tiles.</param>
        /// <param name="scale">Scale.</param>
        /// <param name="layer">Object of map's layer.</param>
        /// <param name="areaSize">Size of destination area.</param>
        public static void PaintEvents(SpriteBatch spriteBatch, Texture2D[] events, Texture2D eventMark, Vector2 position, Int32 tileSize, Single scale, Map.Layer layer, Point areaSize, Boolean centered)
        {
            // Compute tile size for given scale.
            Int32 scaledTileSize = (Int32)((Single)tileSize * scale);

            // Recalculate observer's position to pixels.
            position.X *= (Single)scaledTileSize;
            position.Y *= (Single)scaledTileSize;

            Vector2 offset = new Vector2(position.X, position.Y);

            // Compute indices at the begining of area.
            Int32 xStart = (Int32)(offset.X / scaledTileSize);
            Int32 yStart = (Int32)(offset.Y / scaledTileSize);

            // Compute indices at the and of area.
            Int32 xEnd = xStart + (Int32)(areaSize.X / scaledTileSize) + 3;
            Int32 yEnd = yStart + (Int32)(areaSize.Y / scaledTileSize) + 3;

            // Compute offset for tile.
            Single xOffset = (Int32)offset.X % scaledTileSize;
            Single yOffset = (Int32)offset.Y % scaledTileSize;

            // Helper variables containing current drawing position.
            Int32 posY = 0;
            Int32 posX = 0;

            // Array of tiles.
            //UInt16[,] tiles = layer.Tiles;

            // Compute scale for drawing. Scale for drawing is set to value, which ensures obe tile to be integer size.
            //Single drawScale = (Single)scaledTileSize / (Single)tileSize;

            // Get layer width and height.
            Int32 layerWidth = layer.Width;
            Int32 layerHeight = layer.Height;

            // Crop ending.
            xEnd = Math.Min(xEnd, layerWidth);
            yEnd = Math.Min(yEnd, layerHeight);


            // If position is outside layer area, return without drawing anything.
            if (xStart >= layerWidth)
            {
                return;
            }

            // If position is outside layer area, return without drawing anything.
            if (yStart >= layerHeight)
            {
                return;
            }

            //-- Draw all events on screen.

            // Reset vertical position.
            posY = 0;

            // Go thru all vertical indices.
            for (Int32 hIdx = yStart; hIdx < yEnd; ++hIdx)
            {
                // Reset horizontal position.
                posX = 0;

                // Go thru horizontal indices.
                for (Int32 wIdx = xStart; wIdx < xEnd; ++wIdx)
                {
                    // Get event at position.
                    Byte eventId = layer.Events[wIdx, hIdx];

                    // Ensure event is not empty.
                    if (0 != eventId)
                    {
                        // Compute target position.
                        Single targetX = (Int32)(posX * scaledTileSize) - xOffset;
                        Single targetY = (Int32)(posY * scaledTileSize) - yOffset;

                        // If texture for event isn't empty, draw event.
                        if (events[eventId] != null)
                        {
                            // Compute origin (Origin of an event is at the center of width and bottom.
                            Vector2 origin = new Vector2((Single)(events[eventId].Width / 2), (Single)(events[eventId].Height));

                            if (centered)
                            {
                                origin.Y = events[eventId].Height / 2;
                            }

                            // Compuite position (It's on the center of tile and bottom).
                            Vector2 pos = new Vector2((Single)(targetX + scaledTileSize / 2), (Single)(targetY + scaledTileSize));

                            if (centered)
                            {
                                pos.Y -= scaledTileSize / 2;
                            }

                            // Draw event texture.
                            spriteBatch.Draw(events[eventId], pos, null, Color.White, 0.0f, origin, scale, SpriteEffects.None, 0);
                        }

                        // Draw event mark (black rect with 25% opacity).
                        if (eventMark != null)
                        {
                            // Compute origin (Origin of an event is at the center of width and bottom.
                            Vector2 origin = new Vector2((Single)(eventMark.Width / 2), (Single)(eventMark.Height));

                            // Compuite position (It's on the center of tile and bottom).
                            Vector2 pos = new Vector2((Single)(targetX + scaledTileSize / 2), (Single)(targetY + scaledTileSize));

                            spriteBatch.Draw(eventMark, pos, null, Color.Pink * 0.5f, 0.0f, origin, 1, SpriteEffects.None, 0);
                        }
                    }

                    // Increment horizontal position.
                    posX++;
                }

                // Increment vertical position.
                ++posY;
            }
        }

        public static Int32[,] OptimizeLayer(Map.Layer layer, List<Byte> skipEvents)
        {
            UInt16[,] tiles = layer.Tiles;

            Int32 dimX = tiles.GetLength(0);
            Int32 dimY = tiles.GetLength(1);

            Int32[,] output = new Int32[dimX, dimY];

            // Go thru all vertical indices.
            for (Int32 hIdx = 0; hIdx < dimY; ++hIdx)
            {
                // Go thru horizontal indices.
                for (Int32 wIdx = 0; wIdx < dimX; ++wIdx)
                {
                    // Get tile at position.
                    UInt16 tile = layer.Tiles[wIdx, hIdx];

                    // Ensure it's not empty.
                    if (0 != tile && 0xffff != tile)
                    {
                        if (layer.Events == null || !skipEvents.Contains(layer.Events[wIdx, hIdx]))
                        {
                            output[wIdx, hIdx] = tile | (1 << 16) | (1 << 24);
                        }
                    }
                }
            }

            for (Int32 idx = 0; idx < 3; ++idx)
            {
                try
                {
                    OptimizeHorz(output);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }

                try
                {
                    OptimizeVert(output);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }

            // Go thru all vertical indices.
            for (Int32 hIdx = 0; hIdx < dimY; ++hIdx)
            {
                // Go thru horizontal indices.
                for (Int32 wIdx = 0; wIdx < dimX; ++wIdx)
                {
                    // Get tile at position.
                    Int32 tile = output[wIdx, hIdx];

                    // Ensure it's not empty.
                    if (0 != tile)
                    {
                        Int32 width = (tile >> 16) & 0xff;
                        Int32 height = (tile >> 24) & 0xff;

                        if (width == 1 && height == 1)
                        {
                            output[wIdx, hIdx] = 0;
                        }
                        else
                        {
                            for (Int32 idx = 0; idx < width; ++idx)
                            {
                                for (Int32 idy = 0; idy < height; ++idy)
                                {
                                    if (idx != 0 || idy != 0)
                                    {
                                        output[wIdx + idx, hIdx + idy] = 0xffff;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return output;
        }

        private static void OptimizeHorz(Int32[,] buffer)
        {
            Int32 dimX = buffer.GetLength(0);
            Int32 dimY = buffer.GetLength(1);

            for (Int32 idy = 0; idy < dimY; ++idy)
            {
                for (Int32 idx = 0; idx < dimX; )
                {
                    Int32 tile = buffer[idx, idy];

                    if (tile == 0)
                    {
                        idx++;
                        continue;
                    }

                    Int32 x = tile & 0xff;
                    Int32 y = (tile >> 8) & 0xff;
                    Int32 w = (tile >> 16) & 0xff;
                    Int32 h = (tile >> 24) & 0xff;

                    Int32 idx2 = idx + w;

                    if (idx2 >= dimX)
                        break;

                    Int32 tile2 = buffer[idx2, idy];

                    if (tile2 != 0)
                    {
                        Int32 x2 = tile2 & 0xff;
                        Int32 y2 = (tile2 >> 8) & 0xff;
                        Int32 w2 = (tile2 >> 16) & 0xff;
                        Int32 h2 = (tile2 >> 24) & 0xff;

                        if (x + w == x2 && y == y2 && h == h2)
                        {
                            w = w + w2;

                            buffer[idx2, idy] = 0;

                            tile = x | (y << 8) | (w << 16) | (h << 24);

                            buffer[idx, idy] = tile;
                        }
                    }

                    idx += w;
                }
            }
        }

        private static void OptimizeVert(Int32[,] buffer)
        {
            Int32 dimX = buffer.GetLength(0);
            Int32 dimY = buffer.GetLength(1);

            for (Int32 idx = 0; idx < dimX; ++idx)
            {
                for (Int32 idy = 0; idy < dimY; )
                {
                    Int32 tile = buffer[idx, idy];

                    if (tile == 0)
                    {
                        idy++;
                        continue;
                    }

                    Int32 x = tile & 0xff;
                    Int32 y = (tile >> 8) & 0xff;
                    Int32 w = (tile >> 16) & 0xff;
                    Int32 h = (tile >> 24) & 0xff;

                    Int32 idy2 = idy + h;

                    if (idy2 >= dimY)
                        break;

                    Int32 tile2 = buffer[idx, idy2];

                    if (tile2 != 0)
                    {
                        Int32 x2 = tile2 & 0xff;
                        Int32 y2 = (tile2 >> 8) & 0xff;
                        Int32 w2 = (tile2 >> 16) & 0xff;
                        Int32 h2 = (tile2 >> 24) & 0xff;

                        if (y + h == y2 && x == x2 && w == w2)
                        {
                            h = h + h2;

                            buffer[idx, idy2] = 0;

                            tile = x | (y << 8) | (w << 16) | (h << 24);

                            buffer[idx, idy] = tile;
                        }
                    }

                    idy += h;
                }
            }
        }
    }
}
