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
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ebatianos.Content
{
    /// <summary>
    /// Loader for texture from png file instead of xnb.
    /// </summary>
    public class LoadTexture2DFromPng : ContentLoader.AdditionalType
    {
        private static Boolean _enablePremultiply;
        /// <summary>
        /// Registers type in ContentLoader.
        /// </summary>
        public static void Register(Boolean premultiply)
        {
            RegisterType(typeof(Texture2D), Load, true);

            // Set premultiply flag.
            _enablePremultiply = premultiply;
        }

        /// <summary>
        /// Loads content object.
        /// </summary>
        /// <param name="name">name of resource</param>
        /// <param name="contentLoader">content loader to load additional resources and files</param>
        /// <returns></returns>
        public static Object Load(String name)
        {
            try
            {
                return FromName(name);
            }
            catch (Exception)
            {
                // If file wasn't found, try load asset using native method.
                return ContentLoader.Current.LoadNative<Texture2D>(name);
            }
        }

        public static Texture2D FromName(String name)
        {
            IGraphicsDeviceService deviceService = ContentLoader.Current.GetService<IGraphicsDeviceService>();
            GraphicsDevice device = deviceService.GraphicsDevice;

            // Open png file.
            using (Stream stream = ContentLoader.Current.Open(name + ".png"))
            {
                return FromStream(device, stream);
            }
        }

        public static Texture2D FromStream(GraphicsDevice device, Stream stream)
        {
            Texture2D texture = Texture2D.FromStream(device, stream);
#if !WINDOWSMG
            PremultiplyAlpha(texture);
#endif
            return texture;
        }

        /// <summary>
        /// Converts non-premultiplied texture to premultiplied
        /// </summary>
        /// <param name="texture"></param>
        private static void PremultiplyAlpha(Texture2D texture)
        {
            // Create buffer for texture colors.
            Color[] pixels = new Color[texture.Width * texture.Height];

            // Get texture colors.
            texture.GetData(pixels);

            // Iterate thru colors and multiply color by alpha.
            for (Int32 idx = 0; idx < pixels.Length; idx++)
            {
                Color color = pixels[idx];
                pixels[idx] = new Color(color.R, color.G, color.B) * (color.A / 255f);
            }

            // Set new data to texture.
            texture.SetData(pixels);
        }
    }
}
