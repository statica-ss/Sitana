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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ebatianos.Platform
{
    public class TilesetMask
    {
        public const int MaskSize = 16;

        class Tile
        {
            public Byte[,] mask = new Byte[MaskSize, MaskSize];
        }

        private Tile[,] masks = null;

        public TilesetMask(Texture2D maskTexture)
        {
            Int32 width = maskTexture.Width / MaskSize;
            Int32 height = maskTexture.Height / MaskSize;

            Color[] data = new Color[maskTexture.Width * maskTexture.Height];
            maskTexture.GetData(data);

            masks = new Tile[width, height];

            for (Int32 indexX = 0; indexX < width; ++indexX)
            {
                for (Int32 indexY = 0; indexY < height; ++indexY)
                {
                    masks[indexX, indexY] = new Tile();

                    AnalyzeTile(indexX, indexY, data, maskTexture.Width);
                }
            }
        }

        private void AnalyzeTile(Int32 indexX, Int32 indexY, Color[] data, Int32 stride)
        {
            Byte[,] mask = masks[indexX, indexY].mask;

            Int32 data0 = indexY * MaskSize * stride + indexX * MaskSize;

            for (Int32 y = 0; y < MaskSize; ++y)
            {
                Int32 data1 = data0;

                for (Int32 x = 0; x < MaskSize; ++x)
                {
                    mask[x, y] = (Byte)(data[data1].R > 128 ? 0 : 1);

                    ++data1;
                }

                data0 += stride;
            }
        }

        public Boolean Mask(UInt16 tile, Double offsetX, Double offsetY)
        {
            // Compute source position of tile.
            Int32 srcXOffset = (tile & 0xff);
            Int32 srcYOffset = (tile >> 8);

            Int32 indexX = (Int32)(offsetX * MaskSize) % MaskSize;
            Int32 indexY = (Int32)(offsetY * MaskSize) % MaskSize;

            if (indexX < 0)
            {
                return false;
            }

            if (indexY < 0)
            {
                return false;
            }

            return masks[srcXOffset, srcYOffset].mask[indexX, indexY] > 0;
        }
    }
}
